using UnityEngine;

public class TempEffect : MonoBehaviour, IPoolable
{
    // Describes a temporary one-shot style particle effect that can be easily instanciated through a pooling system.
    public TempEffects ID;
    public float Duration = 2f;

    public ParticleSystem Particles;

    private float timer = 0f;

    public void Begin(Transform pool)
    {
        gameObject.SetActive(true);
        transform.SetParent(pool);
        Play();
    }

    public void End(Transform pool)
    {
        gameObject.SetActive(false);
    }

    public void Play()
    {
        if (Particles.isPlaying)
        {
            Particles.Stop(true);
        }
        Particles.Play(true);

        timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if(timer >= Duration)
        {
            Particles.Stop(true);

            // Return to pool.
            EffectPool.Instance.ReturnToPool(this);
        }
    }
}