using System.Collections.Generic;
using UnityEngine;

public class UnitOptionGO : MonoBehaviour
{
    public static UnitOptionGO Instance;
    [System.Serializable]
    public class UOI
    {
        [SerializeField]
        private UnitOption option;
        public string Name;
        public Sprite Icon;
        public KeyCode DefaultKey;
        public List<UnitOptionInput> Inputs;

        public UnitOption GetOption()
        {
            return option;
        }

    }

    [SerializeField]
    private List<UOI> options = new List<UOI>();

    public Dictionary<UnitOption, UOI> Data = new Dictionary<UnitOption, UOI>();

    public void Awake()
    {
        Instance = this;
        foreach (var op in options)
        {
            if (!Data.ContainsKey(op.GetOption()))
            {
                Data.Add(op.GetOption(), op);
            }
        }
        options.Clear();
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}

public enum UnitOptionInput : byte
{
    POSITION,
    UNIT,
    UNIT_OR_POS
}

public static class UnitOptionInputUtils
{
    public delegate void Output(object obj);

    public static void GetInput(this UnitOptionInput input, Output output)
    {
        switch (input)
        {
            case UnitOptionInput.POSITION:
                UI_PosOrUnitSelect.Instance.GetSelection((isUnit, unit, pos) => { output.Invoke(pos); }, true, false);
                return;
            case UnitOptionInput.UNIT:
                UI_PosOrUnitSelect.Instance.GetSelection((isUnit, unit, pos) => { output.Invoke(unit.gameObject); }, false, true);
                return;
            case UnitOptionInput.UNIT_OR_POS:
                UI_PosOrUnitSelect.Instance.GetSelection(
                    (isUnit, unit, pos) =>
                    {
                        object obj;
                        if (isUnit)                        
                            obj = unit.gameObject;                        
                        else                        
                            obj = pos;                        
                        output.Invoke(obj);
                    }, true, true);
                return;
            default:
                Debug.LogError("Unhandled unit option type: {0}".Form(input));
                return;
        }
    }
}