
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Ship))]
public class ShipLocomotion : NetworkBehaviour
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

    public float MaxThrottle = 10f;
    public float CurrentThrottle = 0f;

    public float MaxTurn = 10f;
    public float CurrentTurn = 0f;

    public void FixedUpdate()
    {
        ClampValues();

        if (!isServer)
            return;

        ApplyThrottle();
        ApplyTurn();
    }

    private void ClampValues()
    {
        MaxThrottle = Mathf.Max(MaxThrottle, 0f);
        MaxTurn = Mathf.Max(MaxTurn, 0f);
        CurrentThrottle = Mathf.Clamp(CurrentThrottle, 0f, MaxThrottle);
        CurrentTurn = Mathf.Clamp(CurrentTurn, -MaxTurn, MaxTurn);
    }

    [Server]
    public void ApplyThrottle()
    {
        Ship.Rigidbody.AddRelativeForce(new Vector2(0f, 1f) * CurrentThrottle, ForceMode2D.Force);
    }

    [Server]
    public void ApplyTurn()
    {
        Ship.Rigidbody.AddTorque(CurrentTurn, ForceMode2D.Force);
    }
}