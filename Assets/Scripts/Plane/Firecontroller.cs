using UnityEngine;

public class FireController : MonoBehaviour
{
    private ParticleSystem fire;

    void Awake()
    {
        fire = GetComponentInChildren<ParticleSystem>();
    }

    public void Extinguish()
    {
        if (fire != null)
        {
            fire.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        Destroy(gameObject);
    }
}