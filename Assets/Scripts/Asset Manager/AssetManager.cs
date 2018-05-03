
using System.Threading;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager Instance;

    public AssetManagerState State
    {
        get
        {
            return _state;
        }
        private set
        {
            _state = value;
        }
    }
    [SerializeField]
    [ReadOnly]
    private AssetManagerState _state;

    public AssetLoadEvent LoadEvent;
    public AssetUnloadEvent UnoadEvent;

    private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    private Thread Thread;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void Load()
    {
        if(State != AssetManagerState.UNLOADED)
        {
            Debug.LogWarning("Cannot load assets when in state {0}".Form(State));
            return;
        }
        State = AssetManagerState.LOADING;

        Debug.Log("Loading all game assets...");
        watch.Restart();

        // Create thread...
        if(Thread != null)
        {
            Thread.Abort();
            Thread = null;
        }
        Thread = new Thread(RunLoad);
        // Start thread.
        Thread.Start();
    }

    private void RunLoad()
    {
        Debug.Log("Running load on other thread...");
        // Lots of work here.

        Thread.Sleep(1000);

        // Done!
        watch.Stop();
        Debug.Log("Loaded all assets in {0} seconds!".Form(watch.Elapsed.TotalSeconds.ToString("n3")));
        State = AssetManagerState.LOADED;
    }

    public void Unload()
    {
        if (State != AssetManagerState.LOADED)
        {
            Debug.LogWarning("Cannot unload assets when in state {0}".Form(State));
            return;
        }
        State = AssetManagerState.UNLOADING;

        Debug.Log("Unloading all game assets...");
        watch.Restart();
        // Unloading is done in the same thread...
        // TODO work here...

        watch.Stop();
        Debug.Log("Unloaded all assets in {0} seconds!".Form(watch.Elapsed.TotalSeconds.ToString("n3")));
        State = AssetManagerState.UNLOADED;
    }
}