using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private GameObject explosionEffectPrefab;

    [Header("Sound")]
    [SerializeField] private AudioClip impactClip;
    [SerializeField, Range(0f, 1f)] private float impactVolume = 0.5f;

    private const string TagFire = "Fire";
    private const string TagSwitchZone = "SwitchZone";

    private Rigidbody rb;
    private bool exploded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.gameObject);
    }

    private void HandleImpact(GameObject obj)
    {
        if (obj.CompareTag(TagSwitchZone)) return;

        Debug.Log($"[WaterProjectile] Va chạm với: {obj.name} (Tag: {obj.tag})");

        if (obj.CompareTag(TagFire))
        {
            FireController fire = obj.GetComponent<FireController>();
            if (fire != null)
            {
                fire.Extinguish();
            }
        }

        Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        PlayImpactSound();

        Destroy(gameObject);
    }

    private void PlayImpactSound()
    {
        if (impactClip == null) return;

        GameObject soundObj = new GameObject("ImpactSound");
        soundObj.transform.position = transform.position;

        AudioSource src = soundObj.AddComponent<AudioSource>();
        src.clip = impactClip;
        src.volume = impactVolume;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = 5f;
        src.maxDistance = 100f;
        src.Play();

        Destroy(soundObj, impactClip.length + 0.1f);
    }
}