
using UnityEngine;

public class WaterTile : MonoBehaviour, IPoolable
{
    public MeshRenderer Renderer;
    public Texture[] Textures;
    public Color Colour;
    public float ChangeTime;

    private float timer;
    private Texture currentTexture;
    private Texture nextTexture;

    public void Begin()
    {
        gameObject.SetActive(true);
        timer = Random.Range(0f, ChangeTime);
        currentTexture = GetRandomTexture();
        do
        {
            nextTexture = GetRandomTexture();
        }
        while (nextTexture == currentTexture);
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    public Texture GetRandomTexture()
    {
        return Textures[Random.Range(0, Textures.Length)];
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(timer > ChangeTime)
        {
            timer -= ChangeTime;
            currentTexture = nextTexture;
            do
            {
                nextTexture = GetRandomTexture();
            }
            while (nextTexture == currentTexture);
        }
        Renderer.material.SetFloat("_Lerp", Mathf.Clamp(timer / ChangeTime, 0f, 1f));
        Renderer.material.SetTexture("_WaterA", currentTexture);
        Renderer.material.SetTexture("_WaterB", nextTexture);
    }
}