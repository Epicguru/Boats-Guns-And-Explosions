using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DamageModel : NetworkBehaviour
{
    public List<DestructiblePart> Parts = new List<DestructiblePart>();
    public Dictionary<DPart, DestructiblePart> PartMap;

    public void Awake()
    {
        ReconstructPartMap();
        ConfigureParts();
    }

    public void ReconstructPartMap()
    {
        if(PartMap == null)
        {
            PartMap = new Dictionary<DPart, DestructiblePart>();
        }
        else
        {
            PartMap.Clear();
        }

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            DPart id = part.ID;

            if (PartMap.ContainsKey(id))
            {
                Debug.LogError("Duplicate part ID in this damage model - Id: {0}, Name: {1}".Form(id, part.Name));
            }
            else
            {
                PartMap.Add(id, part);
            }
        }
    }

    public void ConfigureParts()
    {
        if (Parts == null || Parts.Count == 0)
            return;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            part.Model = this;
        }
    }

    public float GetAverageHealthPercentage()
    {
        if (Parts == null || Parts.Count == 0)
            return 0f;

        float sum = 0f;
        int total = 0;
        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            sum += part.HealthPercentage;
            total++;
        }

        return sum / total;
    }

    public int GetDestroyedCount()
    {
        // Gets the number of destroyed parts.

        if (Parts == null || Parts.Count == 0)
            return 0;

        int count = 0;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            if (part.IsDestroyed)
            {
                count++;
            }
        }

        return count;
    }

    public int GetActiveParts()
    {
        if (Parts == null || Parts.Count == 0)
            return 0;

        int count = 0;
        foreach (var part in Parts)
        {
            if(part != null)
            {
                count++;
            }
        }

        return count;
    }

    public float GetDestroyedPercentage()
    {
        float destroyed = (float)GetDestroyedCount();
        int total = GetActiveParts();

        return destroyed / total;
    }

    public bool HasAllEssentialParts()
    {
        if (Parts == null || Parts.Count == 0)
            return false;

        foreach (var part in Parts)
        {
            if (part == null)
                continue;

            if(part.IsDestroyed && part.IsEssential)
            {
                return false;
            }
        }

        return true;
    }
}