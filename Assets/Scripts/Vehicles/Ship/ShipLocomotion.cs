
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

    public void FixedUpdate()
    {
        ClampThrottles();

        if (!isServer)
            return;

        ApplyThrottle();
    }

    private void ClampThrottles()
    {
        MaxThrottle = Mathf.Max(MaxThrottle, 0f);
        CurrentThrottle = Mathf.Clamp(CurrentThrottle, 0f, MaxThrottle);
    }

    [Server]
    public void ApplyThrottle()
    {
        Ship.Rigidbody.AddRelativeForce(new Vector2(0f, 1f) * CurrentThrottle, ForceMode2D.Force);
    }
}