using System;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileEffector : MonoBehaviour
{
    // Can modify how a projectile interacts with the collider that it attached next to this script.
    // Is called on both client and server, so be careful!
    // Only apply authority in server mode.

    [SerializeField]
    public ProcessHitEffects HitEvent = new ProcessHitEffects();

    public virtual void ProjectileHit(Projectile pr, RaycastHit2D hit)
    {
        if(HitEvent != null)
        {
            HitEvent.Invoke(pr, hit);
        }
    }
}

[Serializable]
public class ProcessHitEffects : UnityEvent<Projectile, RaycastHit2D>
{

}