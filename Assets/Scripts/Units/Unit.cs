
using UnityEngine;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour
{
    [Tooltip("Display name.")]
    public string Name;

    public ushort ID
    {
        get
        {
            return _id;
        }
    }
    [SerializeField]
    [Tooltip("Internal ID")]
    private ushort _id;

    public Faction Faction { get; private set; }
}