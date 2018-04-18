
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

    [Header("Controls")]
    public float MaxThrottle = 10f;
    public float CurrentThrottle = 0f;

    public float MaxTurn = 10f;
    public float CurrentTurn = 0f;

    [Header("Visuals")]
    public ParticleSystem Bubbles;
    public float MaxBubbles = 350f;
    public float MaxBubblesVelocity = 10f;

    [Header("Debug")]
    [ReadOnly]
    public float CurrentSpeed;

    public void Update()
    {
        if (Bubbles == null)
            return;

        Vector2 velocity = GetVelocity();

        float vel = velocity.magnitude;
        float p = Mathf.Clamp(vel / MaxBubblesVelocity, 0f, 1f);

        var em = Bubbles.emission;
        em.rateOverTimeMultiplier = MaxBubbles * p;

        CurrentSpeed = vel;
    }

    public Vector2 GetVelocity()
    {
        if (isServer)
        {
            // Use real rigidbody velocity.
            return Ship.Rigidbody.velocity;            
        }
        else
        {
            // Use NetPosSync's last known velocity from server.
            return Ship.NetPosSync.GetLastVelocity();
        }
    }

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

        MaxBubblesVelocity = Mathf.Max(MaxBubblesVelocity, 0f);
        MaxBubbles = Mathf.Max(MaxBubbles, 0f);
    }

    [Server]
    public void ApplyThrottle()
    {
        Ship.Rigidbody.AddRelativeForce(new Vector2(1f, 0f) * CurrentThrottle, ForceMode2D.Force);
    }

    [Server]
    public void ApplyTurn()
    {
        // Turn speed should be based on forward speed, realistically.
        // But where is the run in realistic?

        Ship.Rigidbody.AddTorque(CurrentTurn, ForceMode2D.Force);
    }
}