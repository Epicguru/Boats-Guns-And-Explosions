using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ObjectPool<T> : MonoBehaviour where T: IPoolable
{
    public Queue<T> Pooled = new Queue<T>();

    public virtual int CurrentlyPooled
    {
        get
        {
            if (Pooled == null)
                return 0;
            return Pooled.Count;
        }
    }
    public int OnLease { get; private set; }
    public int Total { get; private set; }

    public abstract T CreateNew();

    public virtual T GetFromPool()
    {
        if(CurrentlyPooled > 0)
        {
            T fromPool = Pooled.Dequeue();
            OnLease++;

            fromPool.Begin();

            return fromPool;
        }
        else
        {
            // None are pooled, create new
            T newObj = CreateNew();
            if(newObj == null)
            {
                Debug.LogError("Why did an object pool return null from the CreateNew method?");
                return default(T);
            }
            Total++;
            OnLease++;

            newObj.Begin();

            return newObj;
        }
    }

    public virtual void ReturnToPool(T obj)
    {
        if(obj == null)
        {
            return;
        }

        OnLease--;
        obj.End();

        Pooled.Enqueue(obj);
    }
}