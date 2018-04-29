
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Ship))]
public class ShipLocomotion : NetworkBehaviour
{
    private const int MAX_HIT_ITTERATIONS = 10;
    private static RaycastHit2D[] hits = new RaycastHit2D[MAX_HIT_ITTERATIONS];

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
    [SyncVar]
    public float CurrentThrottle = 0f;

    public float MaxTurn = 10f;
    public float CurrentTurn = 0f;

    [Header("Sinking Speed Falloff")]
    public AnimationCurve SinkingSpeed = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    [Header("Visuals")]
    public ParticleSystem Bubbles;
    public float MaxBubbles = 350f;
    public float MaxBubblesVelocity = 10f;

    [Header("Anti-Grouping")]
    public bool AntiGroupingEnabled = true;
    public float MaxDetectionDistance = 2f;
    public float ForceStrength = 500f;

    [Header("Audio")]
    public AudioSource AudioSource;

    [Header("Debug")]
    [ReadOnly]
    public float CurrentSpeed;

    public float ThrottleAmount
    {
        get
        {
            return (float)CurrentThrottle / MaxThrottle;
        }
    }

    public void Update()
    {
        if (Bubbles == null)
            return;

        // Audio, on both client and server.
        UpdateAudio();

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
        DoAntiGrouping();
    }

    private void ClampValues()
    {
        if (!isServer)
            return;

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
        float multiplier = GetAgilityMultiplier();
        Ship.Rigidbody.AddRelativeForce(new Vector2(1f, 0f) * CurrentThrottle * multiplier, ForceMode2D.Force);
    }

    [Server]
    public void ApplyTurn()
    {
        // Turn speed should be based on forward speed, realistically.
        // But where is the run in realistic?

        float multiplier = GetAgilityMultiplier();
        Ship.Rigidbody.AddTorque(CurrentTurn * multiplier, ForceMode2D.Force);
    }

    public float GetAgilityMultiplier()
    {
        return Mathf.Clamp01(SinkingSpeed.Evaluate(Ship.Damage.GetSinkState()));
    }

    [Server]
    public void DoAntiGrouping()
    {
        if (!AntiGroupingEnabled)
            return;

        int hitCount = Physics2D.CircleCastNonAlloc(transform.position, MaxDetectionDistance, Vector2.zero, hits);
        if(hitCount > hits.Length)
        {
            Debug.LogWarning("This '{0}' is within {1} units of {2} colliders, but only capacity for {3} hits. May result in slightly incorrect anti-bunching.".Form(name, MaxDetectionDistance, hitCount, hits.Length));
            // Limit to the max posible hits.
            hitCount = hits.Length;
        }

        for (int i = 0; i < hitCount; i++)
        {
            // Get hit.
            var hit = hits[i];

            // Check that it is a ship.
            Ship ship = hit.transform.GetComponentInParent<Ship>();
            if (ship == null)
                return;

            // Calculate force.
            Vector2 force = transform.position - ship.transform.position;
            force.Normalize();

            // Add force directly away from the other ship.
            Ship.Rigidbody.AddForce(force * ForceStrength, ForceMode2D.Force);
        }
    }

    public void UpdateAudio()
    {
        if (AudioSource == null)
            return;
        if (AudioSource.clip == null)
            return;

        AudioSource.loop = true;
        if (!AudioSource.isPlaying)
        {
            AudioSource.Play();
        }

        // Get current throttle.
        float t = ThrottleAmount;

        AudioSource.volume = Mathf.Clamp(t, 0.1f, 1f);
        AudioSource.pitch = Mathf.Clamp(t * 1.1f, 0.7f, 1.1f);
    }

    public void OnDrawGizmos()
    {
        if (AntiGroupingEnabled)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, MaxDetectionDistance);
        }
    }
}