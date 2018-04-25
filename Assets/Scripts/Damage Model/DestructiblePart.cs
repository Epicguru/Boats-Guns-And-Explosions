﻿
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

    public float Health
    {
        get
        {
            return _health;
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

            _health = value;
        }
    }
    [SerializeField]
    private float _health = 100f;

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

    [Range(0f, 1f)]
    [Tooltip("The point at which the part is considered destroyed beyond repair. 0 means 0 health and 1 means max health.")]
    public float DestroyedLevel = 0f;

    [Tooltip("Is this part/component completely necessary for the DamageModel object to operate/exist/live?")]
    public bool IsEssential = false;

    public float HealthPercentage
    {
        get
        {
            return Mathf.Clamp(Health, 0f, MaxHealth) / Mathf.Max(MaxHealth, 1f);
        }
    }

    public bool IsDestroyed
    {
        get
        {
            return HealthPercentage <= DestroyedLevel;
        }
    }

    public void Update()
    {
        ClampValues();
    }

    private void ClampValues()
    {
        if (Health < 0)
            Health = 0;

        if (MaxHealth < 1)
            MaxHealth = 1;

        if (Health > MaxHealth)
            Health = MaxHealth;
    }
}

public enum DPart : byte
{
    SHIP_ENGINE,
    SHIP_HULL
}