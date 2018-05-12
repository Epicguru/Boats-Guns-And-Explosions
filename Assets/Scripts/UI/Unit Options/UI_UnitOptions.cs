using System.Collections.Generic;
using UnityEngine;

public class UI_UnitOptions : MonoBehaviour
{
    public static UI_UnitOptions Instance;

    public bool Active;
    public Transform Parent;
    public RectTransform OptionsBounds;
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

                spawned.UpdateVisuals();

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

        if(Player.Local != null)
        {
            var array = Unit.CurrentlySelected.ToArray();
            UnitOptionParams[] param = new UnitOptionParams[array.Length];
            Vector2 pos = Random.insideUnitCircle * 20f;
            var p = new UnitOptionParams();
            p.Update(0, true);
            p.Update(1, array[0].gameObject);
            p.Update(2, pos);
            for (int i = 0; i < array.Length; i++)
            {
                param[i] = p;
            }

            Player.Local.UnitOptionExecution.RequestOptionExecution(array, item.Option, param);
        }
    }

    public void DestroySpawned()
    {
        foreach (var item in Spawned)
        {
            UnitOptionItemPool.Instance.ReturnToPool(item);
        }
        Spawned.Clear();
    }
}