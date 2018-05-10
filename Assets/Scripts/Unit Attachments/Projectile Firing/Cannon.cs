using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Attachment))]
public class Cannon : NetworkBehaviour
{
    // Represents some kind of cannon or gun that fires projectiles.
    // It is an attachment.

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

    public float TargetAngle = 0f;

    public ProjectileType ProjectileType = ProjectileType.STANDARD;
    public Transform[] ProjectileSpawnPoints;

    [ServerCallback]
    public void FireFromAnim(AnimationEvent e)
    {
        // Fires a projectile using information supplied by the animation event parameter.
        // Intended for use with the animation event system.

        if(e == null)
        {
            Debug.LogWarning("The animation event parameter is null!");
            return;
        }
        int index = e.intParameter;

        Fire(index);
    }

    public void Fire(int spawnPointIndex)
    {
        if(ProjectileSpawnPoints == null)
        {
            Debug.LogWarning("The projectile spawn points array in this cannon are null.");
            return;
        }
        if (spawnPointIndex < 0 || spawnPointIndex >= ProjectileSpawnPoints.Length)
        {
            Debug.LogWarning("Tried to fire cannon using FireFromAnim, but spawn point index supplied is out of range! Index: {0}, Total Point Count: {1}".Form(index, ProjectileSpawnPoints.Length));
            return;
        }

        Transform pos = ProjectileSpawnPoints[spawnPointIndex];

        Fire(pos);
    }

    [Server]
    public void Fire(Transform pos)
    {
        if (pos == null)
        {
            Debug.LogWarning("Projectile spawn point is null! Was it destroyed or was it never assigned? Cannot spawn projectile!");
            return;
        }

        if (!Attachment.IsAttached)
        {
            Debug.LogWarning("Cannon is not attached to unit, cannot fire!");
            return;
        }

        Faction f;
        if (Attachment.ParentUnit != null)
        {
            f = Attachment.ParentUnit.Faction;
        }
        else
        {
            Debug.LogWarning("Attachment.ParentUnit is null in this cannon, cannot spawn projectile. Attachment sais it is attached, but aparently not!");
            return;
        }

        Projectile.Spawn(ProjectileType, pos.position, pos.right.ToAngle(), f);
    }
}