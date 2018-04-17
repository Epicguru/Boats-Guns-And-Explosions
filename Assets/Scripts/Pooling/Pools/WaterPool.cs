
using UnityEngine;

public class WaterPool : ObjectPool<WaterTile>
{
    public WaterTile Prefab;
    public int Width, Height;

    public override WaterTile CreateNew()
    {
        return Instantiate(Prefab);
    }

    public void Start()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                WaterTile tile = this.GetFromPool();
                tile.transform.localPosition = new Vector3(x + 0.5f, y + 0.5f, 0f);
            }
        }
    }
}