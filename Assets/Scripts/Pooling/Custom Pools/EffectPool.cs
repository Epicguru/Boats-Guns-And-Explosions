using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool Instance;

    public Dictionary<TempEffects, Queue<TempEffect>> Pool = new Dictionary<TempEffects, Queue<TempEffect>>();

    public TempEffect Sparks;
    public TempEffect Explosion;
    public TempEffect DestroyedSparks;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    private TempEffect NewInstance(TempEffects effect)
    {
        switch (effect)
        {
            case TempEffects.SPARKS:
                return Instantiate(Sparks);
            case TempEffects.EXPLOSION:
                return Instantiate(Explosion);
            case TempEffects.DESTROYED_SPARKS:
                return Instantiate(DestroyedSparks);
            default:
                Debug.LogError("No prefab set up for enum type '{0}'".Form(effect));
                return null;
        }
    }

    public TempEffect GetFromPool(TempEffects effect)
    {
        // Dictionary contains...
        if (Pool.ContainsKey(effect))
        {
            // Get from queue.
            var queue = Pool[effect];

            // Are there any in the pool?
            if(queue.Count > 0)
            {
                var pooled = queue.Dequeue();
                pooled.Begin(transform);
                return pooled;
            }
            else
            {
                // Create new.
                var spawned = NewInstance(effect);
                spawned.Begin(transform);
                return spawned;
            }
        }
        else
        {
            // Make new entry and instanciate.
            Pool.Add(effect, new Queue<TempEffect>());

            var spawned = NewInstance(effect);
            spawned.Begin(transform);
            return spawned;
        }
    }

    public void ReturnToPool(TempEffect effect)
    {
        if (effect == null)
            return;

        var id = effect.ID;

        if (Pool.ContainsKey(id))
        {
            // Add to existing sub-pool.
            Pool[id].Enqueue(effect);
            effect.End(transform);
        }
        else
        {
            // Make new sub-pool (puddle?)
            Pool.Add(id, new Queue<TempEffect>());
            Pool[id].Enqueue(effect);
            effect.End(transform);
        }
    }
}

public enum TempEffects : byte
{
    NONE,
    SPARKS,
    EXPLOSION,
    DESTROYED_SPARKS
}