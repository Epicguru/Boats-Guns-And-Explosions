using System.Collections.Generic;
using UnityEngine;

public static class NetworkPrefabs
{
    private static List<GameObject> Buffer = new List<GameObject>();

    public static void Add(GameObject prefab)
    {
        if (!Buffer.Contains(prefab))
        {
            Buffer.Add(prefab);
        }
    }

    public static void Apply()
    {
        if (Buffer.Count == 0)
            return;

        var net = GameObject.FindObjectOfType<NetManager>();

        if(net != null)
        {
            net.spawnPrefabs.Clear();
            net.spawnPrefabs.AddRange(Buffer);
            Buffer.Clear();
        }
        else
        {
            Debug.LogError("Cannot apply networked prefabs right now, could not find the network manager. Wrong scene?");
        }
    }
}