using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    public Animator Anim;
    public Transform Movement;
    public bool ReturnToPool = true;

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
            Pool.Return(GetComponent<PoolableObject>());
        }
    }
}