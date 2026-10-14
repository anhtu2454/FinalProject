using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PlaneEngineSound : MonoBehaviour
{
    [SerializeField] private AudioClip idleClip;    
    [SerializeField] private AudioClip throttleClip;

    private AudioSource audioSource;
    private bool wasThrottling = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
    }

    private void Start()
    {
        PlayClip(idleClip);
    }

    private void Update()
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

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}