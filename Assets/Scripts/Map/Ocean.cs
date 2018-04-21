using UnityEngine;
using UnityEngine.Networking;

public class Ocean : NetworkBehaviour
{
    public MeshRenderer Renderer;
    public int Size = 500;

    [Header("Colour")]
    public Color ShallowWater;
    public Color DeepWater;

    [Header("Waves")]
    public float WaveFrequency;
    [Range(0f, 1f)]
    public float WaveAmplitude;
    public float WaveOffX;
    public float WaveOffY;
    public float WaveVelocityX;
    public float WaveVelocityY;

    public void Update()
    {
        if (Renderer == null)
            return;

        UpdateWaveOffset();

        // Physical size.
        transform.localScale = new Vector3(Size, Size, 1);
        transform.localPosition = new Vector3(0, 0, 1);

        // Shader properties.
        Material m = Renderer.material;
        m.SetColor("_WaterColour", ShallowWater);
        m.SetFloat("_WaveFrequency", WaveFrequency);
        m.SetFloat("_WaveAmplitude", WaveAmplitude);
        m.SetFloat("_WaveOffsetX", WaveOffX);
        m.SetFloat("_WaveOffsetY", WaveOffY);

        float tiling;
        tiling = Mathf.Clamp(20f * (Size / 500f), 1f, 1000f);
        m.SetFloat("_Tiling", tiling);
    }

    private void UpdateWaveOffset()
    {
        WaveOffX += Time.deltaTime * WaveVelocityX;
        WaveOffY += Time.deltaTime * WaveVelocityY;
    }
}