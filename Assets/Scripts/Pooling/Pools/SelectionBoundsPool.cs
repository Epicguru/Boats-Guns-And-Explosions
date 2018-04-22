using UnityEngine;

public class SelectionBoundsPool : ObjectPool<SelectionBounds>
{
    public static SelectionBoundsPool Instance;

    public SelectionBounds Prefab;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public override SelectionBounds CreateNew()
    {
        return Instantiate(Prefab);
    }
}