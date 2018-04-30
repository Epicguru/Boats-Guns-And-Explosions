using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ShipLocomotion))]
[RequireComponent(typeof(Ship))]
public class ShipNavigation : NetworkBehaviour
{
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

    public bool Active
    {
        get
        {
            return _active;
        }
        private set
        {
            if (isServer)
            {
                _active = value;
            }
            else
            {
                Debug.LogError("Cannot set active state when not on server.");
            }
        }
    }
    [SerializeField]
    [SyncVar]
    private bool _active;

    public Vector2 TargetPos
    {
        get
        {
            return _targetPos;
        }
        set
        {
            if (!isServer)
            {
                Debug.LogError("Cannot set target position outside of server!");
            }
            else
            {
                _targetPos = value;
            }
        }
    }
    [SyncVar]
    [SerializeField]
    [ReadOnly]
    private Vector2 _targetPos;

    public float DistanceDeadzone = 1f;
    public float RotationDeadzone = 5f;
    public float MaxThrottleDistance = 20f;
    public AnimationCurve ThrottleCurve = AnimationCurve.Linear(0, 1, 1, 0);

    public float AngleToTarget;
    public float CurrentAngle;

    public void Update()
    {
        ClampValues();

        if(isClient && Player.Local != null)
        {
            // If we are active client...
            if(Ship.Unit.Faction == Player.Local.Faction)
            {
                // If we control/own this ship...
                if (Ship.Unit.IsSelected)
                {
                    // If currently selected by user...
                    // Then draw a cross at the target position.
                    TargetCross.DrawAt(TargetPos);

                    // And a line from the ship to the cross.
                    CameraLines.DrawLine(transform.position, TargetPos);
                }
            }
        }

        if (!isServer)
            return;

        // All server only.

        // If the ship part(s) is/are destroyed, then the engine stops!
        if (IsPhysicallyBroken())
        {
            if (Active)
            {
                Deactivate();
            }
        }

        // If active...
        if (!Active)
            return;

        // If not in deadzone (reached target)...
        float dst = GetDistanceToTarget();
        if (IsInDeadzone(dst))
        {
            ShipLocomotion.CurrentTurn = 0;
            ShipLocomotion.CurrentThrottle = 0;
            return;
        }

        // Adjust throttle based on distance to target.
        ShipLocomotion.CurrentThrottle = GetThrottle(dst);

        // Adjust turn speed based on angle to target.
        ShipLocomotion.CurrentTurn = GetTurn(GetCurrentAngle(), GetAngleToTarget());

        AngleToTarget = GetAngleToTarget();
        CurrentAngle = GetCurrentAngle();
    }

    public bool IsPhysicallyBroken()
    {
        return Ship.DamageModel.IsDestroyed() || Ship.DamageModel.PartMap[DPart.SHIP_ENGINE].IsDestroyed;
    }

    public void GetUnitOptions(List<UnitOption> options)
    {
        // Below: only if not broken.
        if (IsPhysicallyBroken())
        {
            return;
        }

        // Engine state:
        if (Active)
        {
            options.Add(UnitOption.STOP_ENGINE);
        }
        else
        {
            options.Add(UnitOption.START_ENGINE);
        }
    }    

    [Server]
    public void ExecuteOption(OptionAndParams option)
    {
        // Below: only if not broken.
        if (IsPhysicallyBroken())
            return;

        if(option.Option == UnitOption.STOP_ENGINE)
        {
            if (Active)
            {
                Deactivate();
            }
        }

        if (option.Option == UnitOption.START_ENGINE)
        {
            if (!Active)
            {
                Activate();
            }
        }
    }

    public float GetCurrentAngle()
    {
        return transform.eulerAngles.z;
    }

    private float GetTurn(float currentAngle, float targetAngle)
    {
        float shortestAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

        if (Mathf.Abs(shortestAngle) <= RotationDeadzone)
            return 0f;

        if(shortestAngle > 0f)
        {
            // Increase current angle!
            return ShipLocomotion.MaxTurn * GetTurnPercentage(currentAngle, targetAngle);
        }
        else
        {
            // Decrease current angle!
            return -ShipLocomotion.MaxTurn * GetTurnPercentage(currentAngle, targetAngle);
        }
    }

    private float GetTurnPercentage(float currentAngle, float targetAngle)
    {
        return 1f;
    }

    private float GetThrottle(float dst)
    {
        return ShipLocomotion.MaxThrottle * GetThrottlePercentage(dst);
    }

    private float GetThrottlePercentage(float dst)
    {
        float p = Mathf.Clamp(dst / MaxThrottleDistance, 0f, 1f);
        float x = Mathf.Clamp(ThrottleCurve.Evaluate(1f - p), 0f, 1f);

        return x;
    }

    public float GetAngleToTarget()
    {
        float x = TargetPos.x - transform.position.x;
        float y = TargetPos.y - transform.position.y;

        float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        return angle;
    }

    public float GetDistanceToTarget()
    {
        return Vector2.Distance(transform.position, TargetPos);
    }

    public bool IsInDeadzone()
    {
        return IsInDeadzone(GetDistanceToTarget());
    }

    public bool IsInDeadzone(float dst)
    {
        return dst <= DistanceDeadzone;
    }

    private void ClampValues()
    {
        RotationDeadzone = Mathf.Max(RotationDeadzone, 0f);
        MaxThrottleDistance = Mathf.Max(MaxThrottleDistance, 0f);
    }

    [Server]
    public void Activate()
    {
        if (Active)
            return;

        Active = true;
    }

    [Server]
    public void Deactivate()
    {
        if (!Active)
            return;

        Active = false;
        ShipLocomotion.CurrentTurn = 0;
        ShipLocomotion.CurrentThrottle = 0;
    }

    public void OnDrawGizmos()
    {
        if (Active)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, TargetPos);

            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(TargetPos, DistanceDeadzone);

            float targetAngle = GetAngleToTarget() * Mathf.Deg2Rad;
            float size = 3f;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + new Vector2(Mathf.Cos(targetAngle), Mathf.Sin(targetAngle)) * size);

            float currentAngle = GetCurrentAngle() * Mathf.Deg2Rad;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * size);
        }
    }
}