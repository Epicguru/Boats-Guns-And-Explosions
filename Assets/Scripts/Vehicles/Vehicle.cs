
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NetPosSync))]
public abstract class Vehicle : NetworkBehaviour
{
    // All vehicles are physics based moving objects.

    public Rigidbody2D Rigidbody
    {
        get
        {
            if(_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody2D>();
                if(_rigidbody == null)
                {
                    Debug.LogError("Tried to access the rigidbody on this vehicle, but it is missing!");
                }
            }
            return _rigidbody;
        }
    }
    private Rigidbody2D _rigidbody;

    public NetPosSync NetPosSync
    {
        get
        {
            if (_netPosSync == null)
            {
                _netPosSync = GetComponent<NetPosSync>();
                if (_netPosSync == null)
                {
                    Debug.LogError("Tried to access the NetPosSync on this vehicle, but it is missing!");
                }
            }
            return _netPosSync;
        }
    }
    private NetPosSync _netPosSync;

    public float BaseMass = 1f;

    public virtual void Awake()
    {
        ApplyPhysicsSettings();
    }

    public virtual void ApplyPhysicsSettings()
    {
        Rigidbody.useAutoMass = false;
        if(BaseMass <= 0)
        {
            Debug.LogError("This '{0}' has a mass of {1}, invalid mass! Must be greater than 0. Using mass of 0.1".Form(name, BaseMass));
            Rigidbody.mass = 0.1f;
        }
        else
        {
            Rigidbody.mass = BaseMass;
        }
    }
}