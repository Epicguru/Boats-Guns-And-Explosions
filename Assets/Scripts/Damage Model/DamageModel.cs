using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DamageModel : NetworkBehaviour
{
    public List<DestructiblePart> Parts = new List<DestructiblePart>();
    public Dictionary<DPart, DestructiblePart> PartMap;
    public Dictionary<Collider2D, DestructiblePart> ColliderPartMap;

    private PartNetList netData = new PartNetList();

    public Unit Unit
    {
        get
        {
            if(_unit == null)
            {
                _unit = GetComponent<Unit>();
            }
            return _unit;
        }
    }
    private Unit _unit;

    [Range(0.5f, 30f)]
    public float PartSyncRate = 10f;

    [SyncVar]
    public bool CanHit = true;

    private float timer;

    public virtual void Awake()
    {
        ReconstructPartMap();
        ReconstructColliderMap();
        ConfigureParts();
    }

    public virtual void Update()
    {
        if (isServer)
        {
            // On server, send clients data at a rather slow rate.
            // Since the target is to be completely server-authorative, this should not be a problem.
            // Worst case, the client will appear unresponsive.

            timer += Time.unscaledDeltaTime;
            float interval = 1f / PartSyncRate;
            bool run = false;
            while (timer >= interval)
            {
                timer -= interval;
                run = true;
            }

            if (run)
            {
                SynchronizeData();
            }
        }
        else
        {
            // On client update destructible parts based on data sent from the server.
            ReceiveData();
        }
    }

    [Server]
    public void DealExplosionDamage(float maxDamage, DPart origin, float collateralMultiplier = 1f)
    {
        // Simulates an explosion on board based on the relative size and HP of destructible parts.
        // The origin is the center point of the explosion, so will receive full damage.

        if (maxDamage <= 0f)
            return;
        if (origin == DPart.NONE)
            return;

        if (ContainsPart(origin))
        {
            var part = PartMap[origin];
            part.Damage(maxDamage);
        }
        else
        {
            Debug.LogError("Passed explosion origin part '{0}', but that does not exist on this damage model ({1})".Form(origin, name));
        }

        // Calculate collateral damage.
        collateralMultiplier = Mathf.Clamp01(collateralMultiplier);

        foreach (var part in Parts)
        {
            if (part == null)
                continue;
            if (part.ID == origin)
                continue;

            float damage = CalculateCollarteralDamage(maxDamage * collateralMultiplier, part);

            part.Damage(damage);
        }
    }

    [Server]
    public void DealExplosionDamage(float minDamage, float maxDamage)
    {
        float min = Mathf.Min(minDamage, maxDamage);
        float max = Mathf.Max(minDamage, maxDamage);

        if (max <= 0f)
            return;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            float damage = Random.Range(min, max);

            part.Damage(damage);
        }
    }

    [Server]
    public void DealExplosionDamage(float baseDamage)
    {
        if (baseDamage <= 0f)
            return;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            float damage = CalculateCollarteralDamage(baseDamage, part);

            part.Damage(damage);
        }
    }

    [Server]
    public void SynchronizeData()
    {
        netData.Clear();
        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            netData.Add(part.GetNetData());
        }
    }

    [Client]
    public void ReceiveData()
    {
        if (netData == null)
            return;

        foreach (var data in netData)
        {
            if(data.ID != DPart.NONE)
            {
                if (ContainsPart(data.ID))
                {
                    PartMap[data.ID].RecieveNetData(data);
                }
            }
        }
    }

    public virtual float CalculateCollarteralDamage(float baseDamage, DestructiblePart part)
    {
        if (baseDamage <= 0f)
            return 0f;
        if (part == null)
            return 0f;

        float size = Mathf.Clamp01(part.RelativeSize);
        float multiplier = Mathf.Max(part.ExplosionDamageMultipler, 0f);

        float final = baseDamage * size * multiplier;

        return final;
    }

    public bool ContainsPart(DPart part)
    {
        return PartMap.ContainsKey(part);
    }

    public void ReconstructPartMap()
    {
        if(PartMap == null)
        {
            PartMap = new Dictionary<DPart, DestructiblePart>();
        }
        else
        {
            PartMap.Clear();
        }

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            DPart id = part.ID;

            if(id == DPart.NONE)
            {
                Debug.LogWarning("ERROR: Why does '{0}' have a part ID of DPart.NONE??".Form(id));
            }

            if (PartMap.ContainsKey(id))
            {
                Debug.LogError("Duplicate part ID in this damage model - Id: {0}, Name: {1}".Form(id, part.Name));
            }
            else
            {
                PartMap.Add(id, part);
            }
        }
    }

    public void ReconstructColliderMap()
    {
        if (ColliderPartMap == null)
        {
            ColliderPartMap = new Dictionary<Collider2D, DestructiblePart>();
        }
        else
        {
            ColliderPartMap.Clear();
        }

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            Collider2D collider = part.Hitbox;
            if (collider == null)
                continue;

            if (ColliderPartMap.ContainsKey(collider))
            {
                Debug.LogError("Collider hitbox used twice in this damage model - Id: {0}, Name: {1}, Collider Name: {2}".Form(part.ID, part.Name, collider.name));
            }
            else
            {
                ColliderPartMap.Add(collider, part);
            }
        }
    }

    public void ConfigureParts()
    {
        if (Parts == null || Parts.Count == 0)
            return;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            part.Model = this;
        }
    }

    public float GetAverageHealthPercentage()
    {
        if (Parts == null || Parts.Count == 0)
            return 0f;

        float sum = 0f;
        int total = 0;
        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            sum += part.HealthPercentage;
            total++;
        }

        return sum / total;
    }

    public int GetDestroyedCount()
    {
        // Gets the number of destroyed parts.

        if (Parts == null || Parts.Count == 0)
            return 0;

        int count = 0;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            if (part.IsDestroyed)
            {
                count++;
            }
        }

        return count;
    }

    public int GetActiveParts()
    {
        if (Parts == null || Parts.Count == 0)
            return 0;

        int count = 0;
        foreach (var part in Parts)
        {
            if(part != null)
            {
                count++;
            }
        }

        return count;
    }

    public float GetDestroyedPercentage()
    {
        float destroyed = (float)GetDestroyedCount();
        int total = GetActiveParts();

        return destroyed / total;
    }

    public bool HasAllEssentialParts()
    {
        if (Parts == null || Parts.Count == 0)
            return false;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            if(part.IsDestroyed && part.IsEssential)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsDestroyed()
    {
        bool essentials = HasAllEssentialParts();

        return !essentials;
    }
}

public class PartNetList : SyncListStruct<DestructiblePartNetData> { }