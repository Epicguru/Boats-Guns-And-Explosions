using UnityEngine;

public class TargetArrowPool : ObjectPool<TargetArrow>
{
    public static TargetArrowPool Instance;
    public TargetArrow Prefab;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public override TargetArrow CreateNew()
    {
        return Instantiate(Prefab.transform.parent).GetComponentInChildren<TargetArrow>();
    }
}