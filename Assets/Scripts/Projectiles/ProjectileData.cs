using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Projectile Data", menuName = "Projectile")]
public class ProjectileData : ScriptableObject
{
    // Defines a projectiles characteristics.
    // Could add sprite, trail colour/length etc.

    public ProjectileType ID
    {
        get
        {
            return _id;
        }
    }
    [SerializeField]
    private ProjectileType _id;

    [Header("Movement")]
    [Tooltip("The initial starting velocity in units/second")]
    public float Speed = 500f;
    [Range(0f, 50f)]
    [Tooltip("The speed below which the projectile will stop and be destroyed instantly.")]
    public float MinSpeed = 20f;
    [Tooltip("The maximum distance that the projectile can travel from the start point, before being stopped and destroyed instantly.")]
    public float MaxRange = 1000f;
    [Tooltip("The reduction in speed every second, in units.")]
    public float SpeedReduction = 50f;

    [Header("Penetration")]
    [Range(0, 50)]
    [Tooltip("How many layers of colliders can this projectile penetrate through?")]
    public int Penetration = 0;
    [Range(0f, 1f)]
    [Tooltip("The damage penalty for penetrating an object. A multiplier of the current damage.")]
    public float DamageFaloff = 0.7f;
    [Range(0f, 1f)]
    [Tooltip("The speed penalty for penetrating an object. A multiplier of the current speed.")]
    public float SpeedFalloff = 0.8f;

    [Header("Damage")]
    [Tooltip("The type of damage that will be dealt to any DamageModel colliders.")]
    public ProjectileDamageType DamageType = ProjectileDamageType.NORMAL;
    [Tooltip("The base damage. Real damage dealt will be affected by penetration falloff, target armour, damage type etc.")]
    public float Damage = 60f;
    [Range(0f, 1f)]
    [Tooltip("EXPLOSIVE DAMAGE ONLY! The explosion collateral damage multiplier.")]
    public float ExplosionCollateralDamage = 1f;

    [Header("Other")]
    [Tooltip("Can this projectile damage friendly units?")]
    public bool AllowFriendlyFire;

    public static Dictionary<byte, ProjectileData> LoadedData;

    public static void LoadProjectiles()
    {
        if (LoadedData != null)
            return;

        // Load all projectile datas into RAM from the resources.
        LoadedData = new Dictionary<ProjectileType, ProjectileData>();

        var array = Resources.LoadAll<ProjectileData>("Projectiles");

        foreach (var data in array)
        {
            if (!LoadedData.ContainsKey(data.ID))
            {
                LoadedData.Add(data.ID, data);
            }
            else
            {
                Debug.LogError("Duplicate projectile ID {0}! Will not be added.".Form(data.ID));
            }
        }
    }

    public static void Unload()
    {
        // Unload resources from memory...
        if (LoadedData == null)
            return;

        // Unload all projectile data's.
        foreach (var pair in LoadedData)
        {
            if(pair.Value != null)
            {
                Resources.UnloadAsset(pair.Value);
            }
        }

        LoadedData.Clear();
        LoadedData = null;
    }

    public static bool ContainsData(ProjectileType type)
    {
        return ContainsData((byte)type);
    }

    public static bool ContainsData(byte ID)
    {
        return LoadedData != null && LoadedData.ContainsKey(ID);
    }

    public static ProjectileData GetData(ProjectileType type)
    {
        return GetData((byte)type);
    }

    public static ProjectileData GetData(byte ID)
    {
        if (ContainsData(ID))
        {
            return LoadedData[ID];
        }
        else
        {
            return null;
        }
    }
}

public enum ProjectileType : byte
{
    STANDARD
}