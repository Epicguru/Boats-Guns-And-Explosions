
using UnityEngine;

[RequireComponent(typeof(ShipLocomotion))]
public class Ship : Vehicle
{
    [Range(0f, 1f)]
    public float GeneralSize = 0.5f;

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