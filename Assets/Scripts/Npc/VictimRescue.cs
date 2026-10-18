using UnityEngine;


public class VictimRescue : MonoBehaviour
{
    public string helicopterTag = "Helicopter";

    public CargoAttachable hook;

    private Rigidbody rb;
    private bool rescued = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        TryRescue(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        TryRescue(other);
    }

    void TryRescue(Collider other)
    {
        if (rescued) return;

        if (!(other.CompareTag(helicopterTag) || other.transform.root.CompareTag(helicopterTag))) return;

        rescued = true;

        if (hook != null)
        {
            hook.ReleaseCargo();
        }

        Debug.Log($"VictimRescue: '{name}' đã được cứu.");
        Destroy(gameObject);
    }
}