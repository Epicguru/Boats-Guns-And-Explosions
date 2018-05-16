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
    public CanvasGroup Group;
    public float OpenTime = 0.5f;
    public AnimationCurve OpenCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public bool GatheringInput
    {
        get
        {
            return _gatheringInput;
        }
        private set
        {
            _gatheringInput = value;
        }
    }
    [SerializeField]
    [ReadOnly]
    private bool _gatheringInput;
    private int gathered;
    private int workingIndex;
    private int totalToGather;
    private UnitOption option;
    private float openState;

    private List<UnitOptionInput> inputs;
    private List<object> values = new List<object>();

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

        openState = Mathf.Clamp01(openState + (1f / OpenTime) * (Active ? 1f : -1f) * Time.unscaledDeltaTime);
        float p = OpenCurve.Evaluate(openState);
        Group.alpha = p;

        if (GatheringInput)
        {
            if(workingIndex != gathered)
            {
                // We have gathered more input! Move on to the next or terminate.
                if (gathered == totalToGather)
                {
                    // Done!
                    GatheringInput = false;
                    inputs = null;

                    // Execute the option with collected params.
                    UnitOptionParams param = new UnitOptionParams();
                    for (byte i = 0; i < values.Count; i++)
                    {
                        param.Update(i, values[i]);
                    }
                    var array = Unit.CurrentlySelected.ToArray();
                    Player.Local.UnitOptionExecution.RequestOptionExecution(array, option, param);

                    values.Clear();
                }
                else
                {
                    // Move on to the next!
                    workingIndex++;
                    var current = inputs[workingIndex];
                    RunInputCollection(current);
                }
            }
        }
    }

    private void RunInputCollection(UnitOptionInput input)
    {
        input.GetInput((obj) =>
        {
            values.Add(obj);
            gathered++;
        });
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
        if (GatheringInput)
        {
            Debug.LogWarning("Cannot select a new option while already gathering information for the last one. Gathered {0}/{1}".Form(gathered, totalToGather));
            return;
        }

        if(Player.Local != null)
        {
            var inputs = item.Option.GetInputs();
            if(inputs != null && inputs.Count != 0)
            {
                GatheringInput = true;
                totalToGather = inputs.Count;
                gathered = 0;
                workingIndex = 0;
                option = item.Option;

                this.inputs = inputs;

                var first = inputs[0];
                RunInputCollection(first);
            }
            else
            {
                // Possibly get unit generated params?
                var array = Unit.CurrentlySelected.ToArray();
                Player.Local.UnitOptionExecution.RequestOptionExecution(array, item.Option, null);
            }
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