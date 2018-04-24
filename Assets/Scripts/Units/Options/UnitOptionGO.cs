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