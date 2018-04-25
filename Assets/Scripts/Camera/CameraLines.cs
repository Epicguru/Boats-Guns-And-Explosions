
using System.Collections.Generic;
using UnityEngine;

public class CameraLines : MonoBehaviour
{
    public static CameraLines Instance;

    public Material Mat;
    public Color Colour;

    private List<Vector2> points = new List<Vector2>();

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void OnPostRender()
    {
        GL.PushMatrix();
        Mat.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(Colour);
        for (int i = 0; i < points.Count; i += 2)
        {
            int j = i + 1;
            GL.Vertex(points[i]);
            GL.Vertex(points[j]);
        }
        GL.End();
        GL.PopMatrix();

        points.Clear();
    }

    public static void DrawLine(Vector2 start, Vector2 end)
    {
        if (Instance == null)
            return;

        Instance.points.Add(start);
        Instance.points.Add(end);
    }
}