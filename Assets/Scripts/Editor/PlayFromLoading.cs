using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayerFromLoading
{
    // Shift+Z
    private static bool AddedListener = false;
    public const string KEY = "BGAE-LastScene";

    [MenuItem("Game/Start-Stop #z")]
    public static void PlayFromPrelaunchScene()
    {
        if (!AddedListener)
        {
            EditorApplication.playModeStateChanged += StateChange;
            AddedListener = true;
            Debug.Log("Added state change listener.");
        }
        if (EditorApplication.isPlaying == true)
        {
            Debug.Log("Stopping...");
            EditorApplication.isPlaying = false;           
            return;
        }
        var current = EditorSceneManager.GetSceneAt(0);
        EditorPrefs.SetString(KEY, current.path);
        Debug.Log("Current scene is {0} - '{1}'".Form(current.name, current.path));
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/Scenes/Loading.unity");
        EditorApplication.isPlaying = true;
    }

    private static void StateChange(PlayModeStateChange change)
    {
        if(change == PlayModeStateChange.EnteredEditMode)
        {
            // Go back to previous scene.
            Debug.Log(EditorPrefs.GetString(KEY));
            EditorSceneManager.OpenScene(EditorPrefs.GetString(KEY));
        }
    }
}