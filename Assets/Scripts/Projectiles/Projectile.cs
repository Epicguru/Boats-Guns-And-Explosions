using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{
    public const int MAX_COLLISIONS_PER_FRAME = 20;
    public static RaycastHit2D[] Hits = new RaycastHit2D[MAX_COLLISIONS_PER_FRAME];

    public ProjectileHitEvent ServerHit = new ProjectileHitEvent();
    [HideInInspector]
    public List<Collider2D> HitColliders = new List<Collider2D>();

    public ProjectileData Data;
    [SyncVar] public Faction Faction;
    [HideInInspector]
    [SyncVar] public Vector2 StartPos;

    private bool disabled = false;
    private int currentPenetrationCount = 0;
    private float speed;
    private float damage;
    private bool created = false;

    public override void OnStartClient()
    {
        Create();
    }

    public void Start()
    {
        if (isServer)
        {
            Create();
        }
    }

    private void Create()
    {
        if (created)
            return;

        if (Data == null)
        {
            Debug.LogError("Projectile spawned with null data! Why?! ({0})".Form(isServer ? "Server" : "Client"));
            disabled = true;
            return;
        }

        speed = Data.Speed;
        damage = Data.Damage;
        transform.position = StartPos;

        created = true;
    }

    public void Update()
    {
        // On the server, update the authorative position.
        // Also, hit targets!
        UpdatePosition();
        UpdateRange();
        UpdateSpeed();
        UpdateSpeedCutoff();
    }

    public void Disable()
    {
        if (disabled)
            return;

        // Can't destroy on client because it will cause a desync. Just hide the renderer.
        StartCoroutine(DecreaseAlpha());
        disabled = true;

        // If on server, destroy after one second (to allow the tail to catch up to the projectile).
        if (isServer)
        {
            Destroy(gameObject, 1f);
        }
    }

    public IEnumerator DecreaseAlpha()
    {
        var s = GetComponent<SpriteRenderer>();
        while(s.color.a > 0f)
        {
            var c = s.color;
            c.a -= 0.1f;
            s.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateSpeed()
    {
        speed -= Time.deltaTime * Data.SpeedReduction;
    }

    public void UpdateSpeedCutoff()
    {
        if (Data.MinSpeed < 0f)
            Data.MinSpeed = 0f;

        if(Data.Speed <= Data.MinSpeed)
        {
            Disable();
        }
    }

    public void UpdateRange()
    {
        float dst = Vector2.Distance(StartPos, transform.position);

        if(dst > Data.MaxRange)
        {
            Disable();
        }
    }

    public virtual void UpdatePosition()
    {
        // Runs on both client and server, but the server mode is the only one that does authorative actions such as dealing damage.
        // Note that with high latency clients may experience desyc where projectiles appear to hit when don't, but it will be purely visual.

        if (disabled)
            return;

        Vector2 pos = transform.position;
        Vector2 newPos = pos + ((Vector2)transform.right * speed * Time.deltaTime);

        // Update collision!
        Vector2 endPos;
        UpdateCollisionDetection(pos, newPos, out endPos);

        // Move to the new position!
        transform.position = endPos;
    }

    public virtual void UpdateCollisionDetection(Vector2 pos, Vector2 newPos, out Vector2 endPos)
    {
        // The projectile can deal damage and also penetrate through ships' components.
        // If a collider is a trigger, it is ignored.
        // If a collider is part of a damage model, it is considered a penetration hit.
        // If a collider is not a trigger and is not part of a damage model, it is ignored, such as land.

        // TODO prevent an object from being hit more than once.

        // Detect all collisions between our current position and the new position.
        int hits = Physics2D.LinecastNonAlloc(pos, newPos, Hits);

        if(hits > MAX_COLLISIONS_PER_FRAME)
        {
            Debug.LogWarning("Number of detected collisions in projectle exceed the max number of hits per frame, could result in hits not registering! Max hits: {0}, detected hits: {1}".Form(MAX_COLLISIONS_PER_FRAME, hits));
            hits = MAX_COLLISIONS_PER_FRAME;
        }

        for (int i = 0; i < hits; i++)
        {
            var hit = Hits[i];

            // If we have already managed hits with that collider, stop here.
            if (HitColliders.Contains(hit.collider))
            {
                continue;
            }

            if (hit.collider.isTrigger)
            {
                // Is a trigger collider, ignore it.
                continue;
            }

            var model = hit.transform.GetComponentInParent<DamageModel>();
            if(model != null)
            {
                // Check that the model can be hit!
                if (!model.CanHit)
                    continue;

                // Can be hit, it is a model!
                // Indicate that we have hit this collider, to avoid further processing.
                HitColliders.Add(hit.collider);

                // Check to see if it the faction of this projectile matches the model's unit faction.
                if (!Data.AllowFriendlyFire)
                {
                    if (model.Unit != null)
                    {
                        if (model.Unit.Faction == this.Faction)
                        {
                            // We are the same faction, don't allow friendly fire.
                            continue;
                        }
                    }
                }

                // TODO hit it, deal damage, apply force, spawn hit effect.
                ProcessHit(hit, currentPenetrationCount, model);

                // Apply falloff values.
                speed *= Data.SpeedFalloff;
                damage *= Data.DamageFaloff;

                // Increate the penetration count, and if it exceeds max penetration then quit.
                currentPenetrationCount++;
                if(currentPenetrationCount > Data.Penetration)
                {
                    // Stop here, because this projectile cannot penetrate any more and should be destroyed.
                    endPos = hit.point;

                    // Now we want to destroy or hide this projectile.
                    Disable();
                
                    break;
                }
            }
            else
            {
                // Is solid collider with no damage model, ignore it and pass right over it.
                continue;
            }
        }

        endPos = newPos;
    }
    
    public void ProcessHit(RaycastHit2D hit, int penetration, DamageModel model)
    {
        // Can run on either server or client.

        // On server, deals damage, applies forces.
        if (isServer)
        {
            ProcessServerHit(hit, penetration, model);
        }

        // On clients, spawns hit effects and audio ect.
        // TODO apply hit effect and audio.

        // Apply hit effect.
        if(model != null)
        {
            if (model.ColliderPartMap.ContainsKey(hit.collider))
            {
                var hitEffect = model.ColliderPartMap[hit.collider].HitEffect;
                if(hitEffect != TempEffects.NONE)
                {
                    // Spawn hit effect.
                    var effect = EffectPool.Instance.GetFromPool(hitEffect);
                    if(effect != null)
                    {
                        effect.transform.position = hit.point;
                        float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;
                        effect.transform.eulerAngles = new Vector3(0f, 0f, angle);
                    }
                    else
                    {
                        Debug.LogWarning("Tried to spawn effect for projectile hit, but the pool returned null!");
                    }
                }
            }
        }
    }

    [Server]
    private void ProcessServerHit(RaycastHit2D hit, int penetration, DamageModel model)
    {
        if(model == null)
        {
            return;
        }

        if (model.ColliderPartMap.ContainsKey(hit.collider))
        {
            var part = model.ColliderPartMap[hit.collider];

            if (part == null)
                return;

            // Deal damage to that part!
            switch (Data.DamageType)
            {
                case ProjectileDamageType.NORMAL:
                    // Deal damage to only the hit part.
                    part.Damage(damage);

                    break;
                case ProjectileDamageType.EXPLOSIVE:
                    // Do an explosion style damage that will harm parts around the inital hit point.
                    // Does not spawn explosion effect or anything like that.
                    model.DealExplosionDamage(damage, part.ID, Data.ExplosionCollateralDamage);

                    break;
                default:
                    Debug.LogError("Unhandled type of projectile damage!! ({0})".Form(Data.DamageType));
                    break;
            }

            if(ServerHit != null)
            {
                ServerHit.Invoke(hit, penetration, model);
            }
        }
        else
        {
            // Hit a collider that was part of a damage model, but the collider was not mapped.
            // This counted as a penetration hit, but it didn't deal damage. :(
            Debug.LogWarning("Unmapped collider on damage model '{0}'. Collider name: '{1}'. Hit with projectile, but could not deal damage. Please consider making unmapped colliders triggers so that they are not hit.".Form(model.name, hit.collider.name));
            return;
        }
    }

    [Server]
    public static Projectile Spawn(ProjectileType type, Vector2 position, float angle, Faction faction)
    {
        return Spawn(ProjectileData.GetData(type), position, angle, faction);
    }

    [Server]
    public static Projectile Spawn(byte dataID, Vector2 position, float angle, Faction faction)
    {
        return Spawn(ProjectileData.GetData(dataID), position, angle, faction);
    }

    [Server]
    public static Projectile Spawn(ProjectileData data, Vector2 position, float angle, Faction faction)
    {
        if (data == null)
        {
            Debug.LogWarning("Null data supplied, returning null...");
            return null;
        }

        // Create new instance...
        var prefab = Spawnables.Instance.Projectile;
        var instance = Instantiate(prefab);

        // Set position, angle, data, spawn pos, faction...
        instance.transform.position = position;
        instance.transform.eulerAngles = new Vector3(0f, 0f, angle);
        instance.StartPos = position;
        instance.Faction = faction;

        // Spawn on network, syncs with all clients.
        NetworkServer.Spawn(instance.gameObject);

        return instance;
    }
}

public class ProjectileHitEvent : UnityEvent<RaycastHit2D, int, DamageModel>
{

}