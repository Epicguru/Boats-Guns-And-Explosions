using UnityEngine;

public class SelectionBounds : MonoBehaviour, IPoolable
{
    public SpriteRenderer Renderer;

    public void Begin()
    {
        gameObject.SetActive(true);
    }

    public void End(Transform pool)
    {
        gameObject.SetActive(false);
        transform.SetParent(pool);
    }
}