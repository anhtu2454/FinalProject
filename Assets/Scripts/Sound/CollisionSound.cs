using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CollisionSound : MonoBehaviour
{
    [Header("Clip")]
    [SerializeField] private AudioClip hitClip;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float maxVolume = 1f;
    [SerializeField] private float minImpactSpeed = 1.5f;
    [SerializeField] private float maxImpactSpeed = 15f;
    [SerializeField] private float cooldown = 0.15f;

    [Header("3D")]
    [SerializeField] private float minDistance = 10f;
    [SerializeField] private float maxDistance = 100f;

    private AudioSource audioSource;
    private float lastPlayTime = -10f;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (hitClip == null) return;
        if (Time.time - lastPlayTime < cooldown) return;

        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < minImpactSpeed) return;

        float t = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impactSpeed);
        float volume = Mathf.Lerp(0.2f, 1f, t) * maxVolume;

        audioSource.PlayOneShot(hitClip, volume);

        lastPlayTime = Time.time;
    }
}