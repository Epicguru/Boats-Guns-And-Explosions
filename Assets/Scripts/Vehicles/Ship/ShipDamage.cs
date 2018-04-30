
using UnityEngine;
using UnityEngine.Networking;

public class ShipDamage : NetworkBehaviour
{
    [Header("Controls")]
    [SyncVar]
    public float CurrentWater;

    public float MaxWater = 1000;

    [Range(0f, 1f)]
    [Tooltip("The percentage of water in the hull required for the ship to start the sinking process. A value of 0 means that it sinks when completely dry. A value of 1 means that it only starts to sink when completely full.")]
    public float SinkStartThreshhold = 0.5f;

    [Tooltip("The fastest the ship can possibly sink if completely full of water. The real time to sink will normally be quite a bit longer and will depend on how fast the ship is taking on water.")]
    public float MinTimeToSink = 20f;

    [SyncVar]
    public float WaterOntake = 0f;

    [SyncVar]
    private float sinkness;

    public Ship Ship
    {
        get
        {
            if (_ship == null)
            {
                _ship = GetComponent<Ship>();
            }
            return _ship;
        }
    }
    private Ship _ship;

    public float GetWaterPercentage()
    {
        // Gets the amount of water in the hull of the ship.
        // Obviously in a working ship this should be 0.
        // 1 means that the ship is completely submerged and has evolved into a submarine.
        // However, the value does not have to be 1 for the ship to be destroyed.
        // Once the hull starts filling up the ship begins to sink.
        // See GetSinkState for more sinking info.

        ClampValues();
        return CurrentWater / MaxWater;
    }

    public float GetSinkState()
    {
        // Returns how 'sunk' this ship is. 0 means not sunk at all (ideal) and 1 is completely sunk (not good).
        return sinkness;
    }

    public void Update()
    {
        if (isServer)
        {
            // Add/remove water from the hull based on an intake value.
            UpdateWaterOntake();

            // Update how 'sunk' the ship is.
            UpdateSinkness();

            // Make sure nothing is out of range.
            ClampValues();

            // Show smoke when the engine is damaged.
            UpdateEffects();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Ship.DamageModel.DealExplosionDamage(50f);
            }
        }
    }

    [Server]
    private void UpdateEffects()
    {
        // Smoke
        bool smoke = false;
        if (Ship.DamageModel.ContainsPart(DPart.SHIP_ENGINE))
        {
            if (Ship.DamageModel.PartMap[DPart.SHIP_ENGINE].IsDestroyed)
            {
                smoke = true;
            }
        }
        else
        {
            Debug.LogWarning("Ship {1} ({0}) is missing an engine!".Form(Ship.Unit.ID, Ship.Unit.Name));
        }

        if(Ship.Damage.GetSinkState() >= 0.6f)
        {
            smoke = false; // Can't have smoke under water!
        }

        if (smoke != Ship.Effects.SmokeActive)
        {
            Ship.Effects.SmokeActive = smoke;
        }
    }

    [Server]
    private void UpdateWaterOntake()
    {
        CurrentWater += WaterOntake * Time.deltaTime;
    }

    [Server]
    private void UpdateSinkness()
    {
        float p = GetWaterPercentage();

        if(p >= SinkStartThreshhold)
        {
            // Start sinking. The more full of water, the faster the rate of sinking.
            float sinkSpeed = (1f / MinTimeToSink) * (p - SinkStartThreshhold);
            sinkSpeed *= Time.deltaTime;

            sinkness += sinkSpeed;
        }
        else
        {
            // If below the sinking threshhold, the 'unsink' in direct proportion based on the water content.
            sinkness = p * 0.5f;            
        }
    }

    private void ClampValues()
    {
        CurrentWater = Mathf.Clamp(CurrentWater, 0f, MaxWater);
        MaxWater = Mathf.Max(MaxWater, 0.1f);
        sinkness = Mathf.Clamp01(sinkness);
    }
}