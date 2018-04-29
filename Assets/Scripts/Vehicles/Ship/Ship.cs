
using UnityEngine;

[RequireComponent(typeof(ShipUnit))]
[RequireComponent(typeof(ShipLocomotion))]
[RequireComponent(typeof(ShipNavigation))]
[RequireComponent(typeof(ShipDamage))]
public class Ship : Vehicle
{
    [Range(0f, 1f)]
    public float GeneralSize = 0.5f;

    public float MaxWaterOntake = 1000f;

    public ShipLocomotion Locomotion
    {
        get
        {
            if (_locomotion == null)
            {
                _locomotion = GetComponent<ShipLocomotion>();
            }
            return _locomotion;
        }
    }
    private ShipLocomotion _locomotion;

    public ShipNavigation Navigation
    {
        get
        {
            if (_navigation == null)
            {
                _navigation = GetComponent<ShipNavigation>();
            }
            return _navigation;
        }
    }
    private ShipNavigation _navigation;

    public ShipUnit Unit
    {
        get
        {
            if (_unit == null)
            {
                _unit = GetComponent<ShipUnit>();
            }
            return _unit;
        }
    }
    private ShipUnit _unit;

    public ShipDamage Damage
    {
        get
        {
            if(_damage == null)
            {
                _damage = GetComponent<ShipDamage>();
            }
            return _damage;
        }
    }
    private ShipDamage _damage;

    private SpriteRenderer[] Renderers;

    public override void Awake()
    {
        base.Awake();
        if(Materials.Instance != null)
        {
            if(Materials.Instance.ShipShader != null)
            {
                Renderers = GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in Renderers)
                {
                    r.material = Materials.Instance.ShipShader;
                }
            }
        }
    }

    public void Update()
    {
        if (isServer)
        {
            // Server only...
            // Update sinking based on the integrity of the ship hull.

            if (DamageModel.PartMap.ContainsKey(DPart.SHIP_HULL))
            {
                // Integrity value 1 is best. Integrity 0 is the worst. Seriously. It's not good.
                float integrity = Mathf.Clamp01(DamageModel.PartMap[DPart.SHIP_HULL].HealthPercentage);
                MaxWaterOntake = Mathf.Max(MaxWaterOntake, 0f);
                float ontake = Mathf.Clamp((1f - integrity) * MaxWaterOntake, 0f, MaxWaterOntake);
                Damage.WaterOntake = ontake;
            }
            else
            {
                Debug.LogWarning("Ship {0} ({1}) does not contain a SHIP_HULL damage part in the damage model. You need a hull to make a ship work!".Form(Unit.Name, Unit.ID));
            }
        }

        // Client and server:
        // Set shader state based on the sinking state.
        UpdateShaders();
    }

    public void UpdateShaders()
    {
        if (Renderers == null || Renderers.Length == 0)
            return;

        float sink = Damage.GetSinkState();

        foreach (var r in Renderers)
        {
            if(r != null)
            {
                r.material.SetFloat("_Sinkness", sink);
            }
        }
    }

    public override void ApplyPhysicsSettings()
    {
        base.ApplyPhysicsSettings();

        // Linear and angular drag
        Rigidbody.angularDrag = GetAngularDrag();
        Rigidbody.drag = GetLinearDrag();

        // Collision detection and interpolation.
        Rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public virtual float GetLinearDrag()
    {
        // Very small ships have linear drag 3 and largest ships have 10.
        return Mathf.Lerp(3f, 10f, GeneralSize);
    }

    public virtual float GetAngularDrag()
    {
        // Very small ships have angular drag 5 and largest ships have 10.
        return Mathf.Lerp(5f, 10f, GeneralSize);
    }
}