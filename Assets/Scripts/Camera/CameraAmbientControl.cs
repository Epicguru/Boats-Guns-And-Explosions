using UnityEngine;

public class CameraAmbientControl : MonoBehaviour
{
    public Camera Camera;
    public AudioSource Source;

    public float MaxLoudnessSize = 10;
    public float MinLoudnessSize = 50;

    public Vector2 Volume = new Vector2(0f, 1f);
    public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

    public void Update()
    {
        if (Source == null || Camera == null)
            return;

        float size = Camera.orthographicSize;

        if(MaxLoudnessSize < MinLoudnessSize)
        {
            // Zooming in make the sound louder.
            if (size <= MaxLoudnessSize)
            {
                Source.volume = Volume.y;
            }
            else if(size >= MinLoudnessSize)
            {
                Source.volume = Volume.x;
            }
            else
            {
                // Find the lerped value.
                size -= MaxLoudnessSize;
                float p = Mathf.Clamp(size / (MinLoudnessSize - MaxLoudnessSize), 0f, 1f);
                float x = Curve.Evaluate(1f - p);
                Source.volume = Mathf.Lerp(Volume.x, Volume.y, x);
            }
        }
        else
        {
            // Zooming in make the sound quiter
            if (size >= MaxLoudnessSize)
            {
                Source.volume = Volume.y;
            }
            else if (size <= MinLoudnessSize)
            {
                Source.volume = Volume.x;
            }
            else
            {
                // Find the lerped value.
                size -= MinLoudnessSize;
                float p = Mathf.Clamp(size / (MaxLoudnessSize - MinLoudnessSize), 0f, 1f);
                float x = Curve.Evaluate(p);
                Source.volume = Mathf.Lerp(Volume.x, Volume.y, x);
            }
        }
    }
}