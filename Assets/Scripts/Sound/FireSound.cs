using UnityEngine;

public class FireSound : MonoBehaviour
{
    [Header("Clip")]
    public AudioClip burnLoopClip;

    [Header("Settings")]
    [Range(0f, 1f)] public float burnVolume = 0.3f;

    private AudioSource burnSource;

    void Awake()
    {
        burnSource = gameObject.AddComponent<AudioSource>();
        burnSource.clip = burnLoopClip;
        burnSource.loop = true;
        burnSource.volume = burnVolume;
        burnSource.spatialBlend = 1f;
        burnSource.minDistance = 5f;
        burnSource.maxDistance = 50f;
        burnSource.playOnAwake = false;
    }

    void OnEnable()
    {
        if (burnLoopClip != null)
        {
            burnSource.Play();
        }
    }
}