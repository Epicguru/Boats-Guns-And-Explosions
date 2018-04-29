
using System;
using UnityEngine;

[System.Serializable]
public class DestructiblePart
{
    public DPart ID
    {
        get
        {
            return _id;
        }
    }
    [SerializeField]
    [Tooltip("The internal ID of this part. Must be unique within the DamageModel, but not within the whole game.")]
    private DPart _id;

    public string Name = "Part Name";

    public float CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            if (Model == null)
                return;
            if (!Model.isServer)
            {
                Debug.LogError("Cannot set part health when not on server!");
                return;
            }

            _currentHealth = value;
        }
    }
    [SerializeField]
    private float _currentHealth = 100f;

    public float MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            if (Model == null)
                return;
            if (!Model.isServer)
            {
                Debug.LogError("Cannot set part max health when not on server!");
                return;
            }

            _maxHealth = value;
        }
    }
    [SerializeField]
    private float _maxHealth = 100f;

    public Collider2D Hitbox;
    [HideInInspector]
    public DamageModel Model;

    [Header("Damage")]

    [Range(0f, 1f)]
    [Tooltip("How large this part is compared to the overall size of the DamageModel. For example, a helmet would be around 0.1 of an entire person, whereas their clothes would be around 0.8.")]
    public float RelativeSize = 0.2f;

    [Range(0f, 2f)]
    [Tooltip("How much damage is recieved from explosions, in both direct and collateral damage.")]
    public float ExplosionDamageMultipler = 1f;

    [Range(0f, 1f)]
    [Tooltip("The point at which the part is considered destroyed beyond repair. 0 means 0 health and 1 means max health.")]
    public float DestroyedThreshhold = 0f;

    [Tooltip("Is this part/component completely necessary for the DamageModel object to operate/exist/live?")]
    public bool IsEssential = false;

    public DestructiblePartNetData GetNetData()
    {
        return new DestructiblePartNetData() { ID = this.ID, Health = this.CurrentHealth };
    }

    public void RecieveNetData(DestructiblePartNetData data)
    {
        if (data.ID != this.ID)
        {
            Debug.LogWarning("Incorrect net data supplied to part - Local ID: {0}, Net ID: {1}, Local Name: {2}".Form(ID, data.ID, Name));
            return;
        }

        // Set Health
        CurrentHealth = data.Health;
    }

    public float HealthPercentage
    {
        get
        {
            return Mathf.Clamp(CurrentHealth, 0f, MaxHealth) / Mathf.Max(MaxHealth, 1f);
        }
    }

    public bool IsDestroyed
    {
        get
        {
            return HealthPercentage <= DestroyedThreshhold;
        }
    }

    public void Update()
    {
        ClampValues();
    }

    private void ClampValues()
    {
        if (CurrentHealth < 0)
            CurrentHealth = 0;

        if (MaxHealth < 1)
            MaxHealth = 1;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
    }

    public void Damage(float damage)
    {
        if (damage == 0f)
            return;
        if (damage < 0)
            damage *= -1;

        CurrentHealth -= damage;
        ClampValues();
    }

    public void Repair(float health)
    {
        if (health == 0)
            return;

        if (health < 0)
            health *= -1;

        CurrentHealth += health;
        ClampValues();
    }
}

public enum DPart : byte
{
    NONE,
    SHIP_ENGINE,
    SHIP_HULL
}