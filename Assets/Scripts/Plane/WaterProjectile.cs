using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterProjectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private GameObject explosionEffectPrefab;
    private const string TagFire = "Fire";

    private Rigidbody rb;

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
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}