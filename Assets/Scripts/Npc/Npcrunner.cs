using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NPCRunner : MonoBehaviour
{
    [Header("Chạy")]
    [Tooltip("Tốc độ chạy của npc")]
    public float runSpeed = 4f;

    public Vector3 centerOfMassOffset = new Vector3(0, -1f, 0);

    private Rigidbody rb;
    private bool isRunning = false;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;
        animator = GetComponent<Animator>();
    }

    // Gọi từ BreakableWall khi tường vỡ
    public void StartRunning()
    {
        isRunning = true;
        SetRunAnimation(true);
    }

    void FixedUpdate()
    {
        if (!isRunning) return;

        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(runSpeed, v.y, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRunning) return;

        if (collision.gameObject.CompareTag("Mountain")) return;

        StopRunning();
    }

    public void StopRunning()
    {
        isRunning = false;
        SetRunAnimation(false);

        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(0f, v.y, 0f);
    }

    void SetRunAnimation(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("IsRunning", value);
        }
    }
}