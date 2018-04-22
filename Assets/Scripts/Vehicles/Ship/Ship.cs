
using UnityEngine;

[RequireComponent(typeof(ShipUnit))]
[RequireComponent(typeof(ShipLocomotion))]
[RequireComponent(typeof(ShipNavigation))]
public class Ship : Vehicle
{
    [Range(0f, 1f)]
    public float GeneralSize = 0.5f;

    public ShipLocomotion ShipLocomotion
    {
        get
        {
            if (_shipLocomotion == null)
            {
                _shipLocomotion = GetComponent<ShipLocomotion>();
            }
            return _shipLocomotion;
        }
    }
    private ShipLocomotion _shipLocomotion;

    public ShipNavigation ShipNavigation
    {
        get
        {
            if (_shipNavigation == null)
            {
                _shipNavigation = GetComponent<ShipNavigation>();
            }
            return _shipNavigation;
        }
    }
    private ShipNavigation _shipNavigation;

    public ShipUnit ShipUnit
    {
        get
        {
            if(_shipUnit == null)
            {
                _shipUnit = GetComponent<ShipUnit>();
            }
            return _shipUnit;
        }
    }
    private ShipUnit _shipUnit;

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