using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetManager))]
public class AssetManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var m = (target as AssetManager);

        if (GUILayout.Button("Load Game Assets"))
        {
            m.LoadGA();
        }
        if (GUILayout.Button("Unload Game Assets"))
        {
            m.UnloadGA();
        }
    }
}