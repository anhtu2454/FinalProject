using UnityEngine;

public class HelicopterSound : MonoBehaviour
{
    [Header("Engine Clips (loop)")]
    public AudioClip idleClip;
    public AudioClip moveClip;

    [Header("Action Clips")]
    public AudioClip ropeLoopClip;
    public AudioClip hookGrabClip;

    [Header("Settings")]
    [Range(0f, 1f)] public float engineVolume = 0.7f;
    [Range(0f, 1f)] public float ropeVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public float fadeSpeed = 3f;

    AudioSource idleSource;
    AudioSource moveSource;
    AudioSource ropeSource;
    AudioSource sfxSource;

    void Awake()
    {
        idleSource = CreateSource(idleClip, true);
        moveSource = CreateSource(moveClip, true);
        ropeSource = CreateSource(ropeLoopClip, true);
        sfxSource = CreateSource(null, false);

        idleSource.volume = engineVolume;
        moveSource.volume = 0f;
        ropeSource.volume = ropeVolume;

        idleSource.Play();
        moveSource.Play();
    }

    AudioSource CreateSource(AudioClip clip, bool loop)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.clip = clip;
        s.loop = loop;
        s.playOnAwake = false;
        s.spatialBlend = 1f;
        s.minDistance = 10f;
        s.maxDistance = 100f;
        return s;
    }

    void Update()
    {
        HandleEngine();
        HandleRope();
        HandleHook();
    }

    void HandleEngine()
    {
        bool moving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                      Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        float idleTarget;
        float moveTarget;

        if (moving)
        {
            idleTarget = 0f;
            moveTarget = engineVolume;
        }
        else
        {
            idleTarget = engineVolume;
            moveTarget = 0f;
        }

        idleSource.volume = Mathf.MoveTowards(idleSource.volume, idleTarget, fadeSpeed * Time.deltaTime);
        moveSource.volume = Mathf.MoveTowards(moveSource.volume, moveTarget, fadeSpeed * Time.deltaTime);
    }

    void HandleRope()
    {
        bool holdingRope = Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.F);

        if (holdingRope)
        {
            if (!ropeSource.isPlaying)
            {
                ropeSource.Play();
            }
        }
        else
        {
            if (ropeSource.isPlaying)
            {
                ropeSource.Stop();
            }
        }
    }

    void HandleHook()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hookGrabClip != null)
            {
                sfxSource.PlayOneShot(hookGrabClip, sfxVolume);
            }
        }
    }
}