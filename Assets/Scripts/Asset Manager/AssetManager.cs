using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssetManager : MonoBehaviour
{
    // Loads and disposes of assets when they are required throughout the execution of the program.
    // It does not actually do any loading itself, just manages other classes that do the real work.

    public static AssetManager Instance;

    public AssetLoadEvent LoadGameAssets = new AssetLoadEvent();
    public AssetUnloadEvent UnloadGameAssets = new AssetUnloadEvent();

    [ReadOnly]
    public bool IsLoading;

    [ReadOnly]
    public bool IsUnloading;

    [ReadOnly]
    public float Progress;

    private int total;
    private List<AsyncOperation> operations;
    private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    private bool loaded;

    public void LoadGA()
    {
        if (IsLoading)
        {
            return;
        }
        if (loaded)
        {
            return;
        }
        if (IsUnloading)
        {
            return;
        }

        // Start stopwatch
        watch.Restart();

        IsLoading = true;

        // Load all data that is requested from 'LoadGameData'.
        Progress = 0;

        // Start the loading process.
        operations = new List<AsyncOperation>();
        LoadGameAssets.Invoke(operations);

        total = MakeSum(operations);
    }

    public void UnloadGA()
    {
        if (IsLoading)
        {
            return;
        }
        if (!loaded)
        {
            return;
        }
        if (IsUnloading)
        {
            return;
        }
        IsUnloading = true;

        // Unload all assets.
        Debug.Log("Unloading game assets...");
        watch.Restart();

        UnloadGameAssets.Invoke();

        double time = watch.Elapsed.TotalSeconds;
        watch.Stop();
        watch.Reset();

        Debug.Log("Unloaded all game assets in {0} seconds.".Form(time));
        IsUnloading = false;
        loaded = false;
    }

    public void Update()
    {
        if (!IsLoading)
            return;

        // Update loading state.
        Progress = MakeAverage(operations, total);

        if (IsDone(operations))
        {
            IsLoading = false;
            loaded = true;

            Debug.Log("Done loading!");

            // Stop stopwatch
            watch.Stop();
            double time = watch.Elapsed.TotalSeconds;
            watch.Reset();
            Debug.Log("Took {0} seconds to load all game assets.".Form(time));

            operations.Clear();
            operations = null;

            total = 0;
            Progress = 0f;
        }
    }

    private int MakeSum(List<AsyncOperation> ops)
    {
        int total = 0;
        for (int i = 0; i < ops.Count; i++)
        {
            if (ops[i] != null)
            {
                total++;
            }
        }
        return total;
    }

    private float MakeAverage(List<AsyncOperation> ops, int total)
    {
        float sum = 0f;
        foreach (var op in ops)
        {
            if(op != null)
            {
                sum += op.progress;
            }
        }

        return sum / total;
    }

    private bool IsDone(List<AsyncOperation> ops)
    {
        foreach (var op in ops)
        {
            if (op != null && !op.isDone)
                return false;
        }
        return true;
    }

    private void Awake()
    {
        Instance = this;
        IsLoading = false;
        IsUnloading = false;
        loaded = false;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}

[Serializable]
public class AssetLoadEvent : UnityEvent<List<AsyncOperation>>
{

}

[Serializable]
public class AssetUnloadEvent : UnityEvent
{

}