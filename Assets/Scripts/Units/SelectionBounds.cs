using UnityEngine;

public class SelectionBounds : MonoBehaviour, IPoolable
{
    public SpriteRenderer Renderer;

    public void Begin(Transform pool)
    {
        gameObject.SetActive(true);
        transform.SetParent(pool);
    }

    public void End(Transform pool)
    {
        gameObject.SetActive(false);
        transform.SetParent(pool);
    }
}