using UnityEngine;

public class Materials : MonoBehaviour
{
    public static Materials Instance;

    public Material ShipShader;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}