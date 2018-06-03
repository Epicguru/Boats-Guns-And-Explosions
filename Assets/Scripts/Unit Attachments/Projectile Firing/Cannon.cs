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

    public bool IsFiring
    {
        get
        {
            return _isFiring;
        }
        set
        {
            if (!isServer)
            {
                Debug.LogError("Cannot set firing flag when not on server!");
                return;
            }
            if(_isFiring != value)
            {
                _isFiring = value;
                if (value)
                {
                    Active = true;
                }
            }
        }
    }
    [Header("Shooting")]
    [SerializeField]
    [ReadOnly]
    [SyncVar]
    private bool _isFiring;

    [Header("Turning")]
    public bool Active;
    public float TargetAngle = 0f;
    public float MaxRotationSpeed = 360f;

    [Header("Projectiles")]
    public ProjectileType ProjectileType = ProjectileType.STANDARD;
    public Transform[] ProjectileSpawnPoints;

    [Header("Audio")]
    public AudioSource Source;
    public AudioClip[] Clips;

    [Header("Animation")]
    public Animator Anim;
    public string FireBool = "Fire";

    [Header("References")]
    public PoolableObject Shockwave;

    private void Start()
    {
        if(Source == null)
        {
            Source = GetComponentInChildren<AudioSource>();
        }
        if(Anim == null)
        {
            Anim = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (isServer)
        {
            RotateToTarget();
        }
        // On both client and server: Cause firing animation.
        // On server this actually fires projectiles.
        if(Anim != null)
        {
            Anim.SetBool(FireBool, IsFiring);
        }
    }

    public void PlayShotSound()
    {
        if(Source == null)
        {
            Debug.LogWarning("Audio source on this cannon '{0}' is null, cannot play shot sound!".Form(gameObject.name));
            return;
        }

        if(Clips == null)
        {
            Debug.LogWarning("Audio clip array for this cannon '{0}' is null, cannot play shot sound!".Form(gameObject.name));
            return;
        }

        int index = Random.Range(0, Clips.Length);
        AudioClip clip = Clips[index];

        if(clip == null)
        {
            Debug.LogWarning("Audio clip from array at index {1} for this cannon '{0}' is null, cannot play shot sound!".Form(gameObject.name, index));
            return;
        }

        Source.clip = clip;
        Source.Play();
    }

    [Server]
    private void RotateToTarget()
    {
        if (Active)
        {
            var angles = transform.eulerAngles;
            float rot = angles.z;

            float movement = Mathf.DeltaAngle(rot, TargetAngle);

            if (movement == 0f)
            {
                // No reason to move the cannon if we are already at the target angle.
                return;
            }

            float change = 0f;
            if (movement > 0f)
            {
                // The target angle is 'above' our current angle, move upwards.
                change = Time.deltaTime * MaxRotationSpeed;
                if (change > movement)
                {
                    change = movement;
                }
            }
            else
            {
                // The target angle is 'below' our current angle, move downwards.
                change = Time.deltaTime * MaxRotationSpeed * -1f;
                if (change < movement)
                {
                    change = movement;
                }
            }

            rot += change;
            angles.z = rot;
            transform.eulerAngles = angles;
        }
        else
        {
            var angles = transform.localEulerAngles;
            float z = angles.z;
            float movement = Mathf.DeltaAngle(z, 0f);
            if(movement == 0f)
            {
                return;
            }

            float change = 0f;
            if (movement > 0f)
            {
                // The target angle is 'above' our current angle, move upwards.
                change = Time.deltaTime * MaxRotationSpeed;
                if (change > movement)
                {
                    change = movement;
                }
            }
            else
            {
                // The target angle is 'below' our current angle, move downwards.
                change = Time.deltaTime * MaxRotationSpeed * -1f;
                if (change < movement)
                {
                    change = movement;
                }
            }

            z += change;
            angles.z = z;
            transform.localEulerAngles = angles;
        }
    }

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

        // Play shot audio, on server and clients...
        PlayShotSound();

        // Spawn shockwave effect...
        if (!(index < 0 || index >= ProjectileSpawnPoints.Length))
        {
            var spawnPoint = ProjectileSpawnPoints[index];
            var forward = spawnPoint.right * 0.1f;
            var point = spawnPoint.position + forward;

            Pool.Get(Shockwave, point);
        }

        // Only do the real firing on the server.
        if (isServer)
        {
            Fire(index);
        }
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
            Debug.LogWarning("Tried to fire cannon using FireFromAnim, but spawn point index supplied is out of range! Index: {0}, Total Point Count: {1}".Form(spawnPointIndex, ProjectileSpawnPoints.Length));
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