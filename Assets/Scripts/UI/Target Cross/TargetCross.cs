using UnityEngine;

public class TargetCross : MonoBehaviour
{
    public static TargetCross Instance;

    public Mesh MeshToDraw;
    public Material Material;
    public Vector3 Angle;

    private Quaternion angle;

    public void Awake()
    {
        Instance = this;

        angle = Quaternion.Euler(Angle);
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public static void DrawAt(Vector2 center)
    {
        if (Instance == null)
            return;

        Graphics.DrawMesh(Instance.MeshToDraw, center, Instance.angle, Instance.Material, 0);
    }
}