using UnityEngine;

public class Spawnables : MonoBehaviour
{
    public static Spawnables Instance;

    public Projectile Projectile;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}