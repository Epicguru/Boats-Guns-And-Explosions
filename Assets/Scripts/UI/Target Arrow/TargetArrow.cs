using UnityEngine;

public class TargetArrow : MonoBehaviour, IPoolable
{
    public Animator Anim;
    public Transform Movement;
    public bool ReturnToPool = true;

    public void Begin(Transform pool)
    {
        Movement.transform.SetParent(pool);
        Movement.gameObject.SetActive(true);
    }

    public void PlaceAt(Vector2 position)
    {
        Movement.transform.position = position;
        Anim.SetTrigger("Place");
    }

    public void Remove()
    {
        Anim.SetTrigger("Remove");
    }

    private void FromAnimRemove()
    {
        // Remove from world, return to pool.
        if (ReturnToPool)
        {
            TargetArrowPool.Instance.ReturnToPool(this);
        }
    }

    public void End(Transform pool)
    {
        Movement.gameObject.SetActive(false);
        Movement.transform.SetParent(pool);
    }
}