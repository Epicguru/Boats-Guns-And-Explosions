using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Attachment))]
public class ActiveDefenceSystem : NetworkBehaviour
{
    public Attachment Attachment
    {
        get
        {
            if(_attachment == null)
            {
                _attachment = GetComponent<Attachment>();
            }
            return _attachment;
        }
    }
    private Attachment _attachment;

    public CircleCollider2D Collider;
    public Transform Field;

    public float Range = 5f;
    public Transform Top;

    public float LineTime = 1f;
    public Color Start, End;

    private List<KeyValuePair<Vector2, Vector2>> lines = new List<KeyValuePair<Vector2, Vector2>>();
    private List<float> times = new List<float>();

    public void PlayShootdownAnim(Vector2 targetPos)
    {
        // On both clients and servers.

        float angleToTarget = ((Vector2)transform.position - targetPos).ToAngle();
        Top.transform.eulerAngles = new Vector3(0f, 0f, angleToTarget);

        // TODO audio.

        // Draw an ADS line.
        PostLine(transform.position, targetPos);
    }

    public void PostLine(Vector2 start, Vector2 end)
    {
        if (start == end)
            return;

        lines.Add(new KeyValuePair<Vector2, Vector2>(start, end));
        times.Add(0f);
    }

    public void ProjectileIntercepted(Projectile pr, RaycastHit2D hit)
    {
        // Called on both client and server.
        // Disable the projectile.
        // On server, this is authorative.
        // On client, this hides it, and could lead to visual desync. Oh well.


        // Ignore if not attached.
        if (!Attachment.IsAttached)
        {
            return;
        }

        // Do not shoot down our own projectiles!
        if (Attachment.ParentUnit.Faction == pr.Faction)
        {
            return;
        }

        pr.Disable(hit.point);

        // Play the shoot down effect, and spawn a shot down particle effect.
        PlayShootdownAnim(hit.point);

        var particle = EffectPool.Instance.GetFromPool(TempEffects.DESTROYED_SPARKS);
        particle.transform.position = hit.point;
        particle.transform.eulerAngles = pr.transform.eulerAngles;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            PlayShootdownAnim((Vector2)transform.position + (Random.insideUnitCircle * Range));
        }

        // Update lines and draw them.
        for (int i = 0; i < lines.Count; i++)
        {
            Color c = Color.Lerp(Start, End, times[i] / LineTime);
            CameraLines.DrawLine(lines[i].Key, lines[i].Value, c);

            times[i] += Time.deltaTime;
            if(times[i] >= LineTime)
            {
                lines.RemoveAt(i);
                times.RemoveAt(i);
                i--;
            }
        }

        Collider.radius = Range;
        float x = (Range / 3f);
        Field.transform.localScale = new Vector3(x, x, 1f);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}