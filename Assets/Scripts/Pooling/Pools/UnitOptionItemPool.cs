
public class UnitOptionItemPool : ObjectPool<UI_UnitOptionItem>
{
    public static UnitOptionItemPool Instance;
    public UI_UnitOptionItem Prefab;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public override UI_UnitOptionItem CreateNew()
    {
        return Instantiate(Prefab);
    }
}