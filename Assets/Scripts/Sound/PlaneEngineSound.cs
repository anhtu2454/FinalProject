using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaneEngineSound : MonoBehaviour
{
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioClip throttleClip;

    [Header("Shift")]
    [SerializeField] private AudioClip shiftClip;
    [SerializeField, Range(0f, 1f)] private float shiftVolume = 1f;

    private AudioSource audioSource;
    private AudioSource shiftSource;
    private bool wasThrottling = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        shiftSource = gameObject.AddComponent<AudioSource>();
        shiftSource.loop = false;
        shiftSource.playOnAwake = false;
        shiftSource.spatialBlend = audioSource.spatialBlend;
        shiftSource.minDistance = audioSource.minDistance;
        shiftSource.maxDistance = audioSource.maxDistance;
        shiftSource.rolloffMode = audioSource.rolloffMode;
    }

    private void Start()
    {
        PlayClip(idleClip);
    }

    private void Update()
    {
        HandleEngine();
        HandleShift();
    }

    private void HandleEngine()
    {
        bool isThrottling = Input.GetKey(KeyCode.Space);

        if (isThrottling != wasThrottling)
        {
            if (isThrottling)
            {
                PlayClip(throttleClip);
            }
            else
            {
                PlayClip(idleClip);
            }

            wasThrottling = isThrottling;
        }
    }

    private void HandleShift()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (shiftClip != null)
            {
                shiftSource.PlayOneShot(shiftClip, shiftVolume);
            }
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}