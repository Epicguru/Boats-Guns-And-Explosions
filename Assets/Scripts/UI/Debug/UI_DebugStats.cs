using UnityEngine;
using UnityEngine.UI;

public class UI_DebugStats : MonoBehaviour
{
    public Text Text;
    [ReadOnly]
    public int FPS;

    private int frames;
    private float time;

    public void Update()
    {
        frames++;
        time += Time.unscaledDeltaTime;

        bool ticked = false;
        while(time >= 1f)
        {
            time -= 1f;
            ticked = true;
        }

        if (ticked)
        {
            FPS = frames;
            frames = 0;
        }

        Text.text = "FPS: {0}".Form(FPS);
    }
}