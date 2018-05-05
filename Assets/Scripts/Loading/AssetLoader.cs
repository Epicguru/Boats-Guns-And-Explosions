using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class AssetLoader : MonoBehaviour
{
    public static AssetLoader Instance;

    public UI_LoadingMenu UI;
    public string GameScene;

    [Header("Network")]
    public bool Server;
    public bool Client;
    public bool Host
    {
        get
        {
            return Client && Server;
        }
    }

    public string IP = "localhost";
    public int Port = 7777;
    [ReadOnly]
    public bool LoadedStatic = false;

    private delegate void Run();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {        
        StartCoroutine(LoadStaticAssets());
    }

    private IEnumerator LoadStaticAssets()
    {
        if (!LoadedStatic)
        {
            LoadedStatic = false;

            List<KeyValuePair<string, Run>> actions = new List<KeyValuePair<string, Run>>();
            actions.Add(new KeyValuePair<string, Run>("Loading Projectiles...", () => { ProjectileData.LoadProjectiles(); }));

            int total = actions.Count;

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            // Loop and execute...
            for (int i = 0; i < total; i++)
            {
                var pair = actions[i];
                // Execute...
                UI.Title = pair.Key;
                UI.Percentage = i / total;
                yield return null;
                watch.Restart();
                pair.Value.Invoke();
                watch.Stop();
                Debug.Log("'{0}' - Took {1} milliseconds.".Form(pair.Key, watch.ElapsedMilliseconds));
            }

            LoadedStatic = true;
            StartCoroutine(LoadScene());
        }
    }

    private void UnloadStaticAssets()
    {
        if (!LoadedStatic)
        {
            // Projectiles...
            ProjectileData.Unload();

            LoadedStatic = false;
        }
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
        // Start as host.
        var net = FindObjectOfType<NetManager>();
        if(net == null)
        {
            Debug.LogError("Could not find network manager!");
        }
        else
        {
            net.networkPort = this.Port;
            if (Host)
            {
                net.StartHost();
                Debug.Log("Started game as host on port {0} ...".Form(net.networkPort));
            }
            else if (Server)
            {
                net.StartServer();
                Debug.Log("Started game as standalone server on port {0} ...".Form(net.networkPort));
            }
            else if(Client)
            {
                net.networkAddress = this.IP;
                net.StartClient();
                Debug.Log("Started game as client connected to remote {0} on {1}".Form(net.networkAddress, net.networkPort));
            }
            else
            {
                Debug.LogError("Impropper network setup, neither client nor server!");
            }
        }        
    }
}