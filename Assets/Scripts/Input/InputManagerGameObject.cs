using System.Collections.Generic;
using UnityEngine;

public class InputManagerGameObject : MonoBehaviour
{
    [SerializeField]
    private List<NameKeyBinding> Bindings;

    public void SaveToFile()
    {
        Dictionary<string, KeyCode[]> dic = new Dictionary<string, KeyCode[]>();
        foreach (var item in Bindings)
        {
            dic.Add(item.Name, item.Keys);
        }
        GameIO.ObjectToResource(dic, GameIO.DefaultInputPath);
        Debug.Log(string.Format("Saved {0} inputs to '{1}'", Bindings.Count, GameIO.FullResourcePath(GameIO.DefaultInputPath)));
    }

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Bindings = null;
    }

    public void Update()
    {
        InputManager.Update();
    }
}

[System.Serializable]
public class NameKeyBinding
{
    public string Name;
    public KeyCode[] Keys;
}