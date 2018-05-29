using UnityEngine;

public class Spawnables : MonoBehaviour
{
    public static Spawnables Instance;

    public PoolableObject UnitSelectionBounds;
    public Projectile BaseProjectile;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}