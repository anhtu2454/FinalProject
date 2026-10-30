using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Meteor : MonoBehaviour
{
    public float fallSpeed = 15f;

    public float lifeTime = 15f;

    public GameObject explosionPrefab;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}