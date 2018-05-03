using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public UI_LoadingMenu UI;
    public string GameScene;

    [ReadOnly]
    public bool LoadedStatic = false;

    public void Start()
    {
        // First load static assets, and finally the scene.
        StartCoroutine(LoadStaticAssets());
    }

    private IEnumerator LoadStaticAssets()
    {
        LoadedStatic = false;
        const int TOTAL = 1;
        float current = 0;

        // Projectiles...
        UI.Title = "Loading Projectiles...";
        UI.Percentage = current / TOTAL;
        yield return null;
        ProjectileData.LoadProjectiles();
        current++;

        LoadedStatic = true;
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        UI.Title = "Loading Map...";
        UI.Percentage = 0f;
        var op = SceneManager.LoadSceneAsync(GameScene);
        while (!op.isDone)
        {
            UI.Percentage = op.progress;
            yield return null;
        }
    }
}