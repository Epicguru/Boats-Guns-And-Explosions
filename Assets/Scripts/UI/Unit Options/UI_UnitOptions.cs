using System.Collections.Generic;
using UnityEngine;

public class UI_UnitOptions : MonoBehaviour
{
    public static UI_UnitOptions Instance;

    public bool Active;
    public Transform Parent;
    public List<UI_UnitOptionItem> Spawned = new List<UI_UnitOptionItem>();
    public GameObject NoOptions;

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Update()
    {
        if (NoOptions.activeSelf && !Active)
            NoOptions.SetActive(false);
        Parent.gameObject.SetActive(Active);
    }

    public void SpawnOptions(Dictionary<UnitOption, int> options)
    {
        DestroySpawned();

        if(options == null || options.Count == 0)
        {
            // No options...
            NoOptions.SetActive(true);
            return;
        }
        else
        {
            NoOptions.SetActive(false);
            int index = 0;
            foreach (var pair in options)
            {
                if (pair.Value <= 0)
                    continue;

                var spawned = UnitOptionItemPool.Instance.GetFromPool();

                // Set option data to be shown in UI.
                spawned.Option = pair.Key;
                spawned.Key = pair.Key.GetInputKey(index);
                spawned.Count = pair.Value;
                spawned.Icon = pair.Key.GetIcon();
                spawned.Title = pair.Key.GetString();
                spawned.Options = this;

                // Parent to the UI.
                spawned.transform.SetParent(Parent);

                this.Spawned.Add(spawned);

                index++;
            }
        }
    }

    public void Selected(UI_UnitOptionItem item)
    {
        if (item == null)
            return;

        Debug.Log(item.Option);
    }

    public void DestroySpawned()
    {
        foreach (var item in Spawned)
        {
            Destroy(item.gameObject);
        }
        Spawned.Clear();
    }
}