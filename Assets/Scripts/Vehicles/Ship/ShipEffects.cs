using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Ship))]
public class ShipEffects : NetworkBehaviour
{
    public Ship Ship
    {
        get
        {
            if(_ship == null)
            {
                _ship = GetComponent<Ship>();
            }
            return _ship;
        }
    }
    private Ship _ship;

    [Header("Controls")]
    [SyncVar]
    public bool SmokeActive;

    [Header("References")]
    public ParticleSystem Smoke;

    public void Awake()
    {
        if(Smoke != null)
        {
            Smoke.Stop();
        }
        else
        {
            ErrorMissing("Smoke");
        }
    }

    public void Update()
    {
        // Smoke
        UpdateEnabledState(Smoke, SmokeActive);
    }

    private void UpdateEnabledState(ParticleSystem system, bool active)
    {
        if (system == null)
            return;

        if (active)
        {
            if (!system.isPlaying)
            {
                system.Play(true);
            }
        }
        else
        {
            if (system.isPlaying)
            {
                system.Stop(true);
            }
        }
    }

    private void ErrorMissing(string name)
    {
        Debug.LogError("Missing essential particle effect '{0}' on ship {1}({2}), GO name {3}".Form(name, Ship.Unit.Name, Ship.Unit.ID, gameObject.name));
    }
}