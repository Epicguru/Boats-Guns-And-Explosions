using UnityEngine;

public class TargetCross : MonoBehaviour
{
    public static TargetCross Instance;

    public Mesh MeshToDraw;
    public Material Material;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public static void DrawAt(Vector2 center)
    {
        if (Instance == null)
            return;

        Graphics.DrawMeshNow(Instance.MeshToDraw, center, Quaternion.identity);
    }
}