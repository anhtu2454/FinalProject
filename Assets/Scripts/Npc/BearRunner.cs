using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BearRunner : MonoBehaviour
{
    [Header("Chạy")]
    [Tooltip("Tốc độ chạy của gấu")]
    public float runSpeed = 4f;

    public Vector3 centerOfMassOffset = new Vector3(0, -1f, 0);

    private Rigidbody rb;
    private bool isRunning = false;

    // 1 = chạy sang phải, -1 = chạy sang trái
    private float direction = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;
    }


    public void StartRunning()
    {
        isRunning = true;
    }

    void FixedUpdate()
    {
        if (!isRunning) return;

        Vector3 v = rb.linearVelocity;
        rb.linearVelocity = new Vector3(direction * runSpeed, v.y, 0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRunning) return;

        if (collision.gameObject.CompareTag("Mountain")) return;

        if (collision.gameObject.CompareTag("Victim"))
        {
            GameManager.Instance.FailMission();
        }

        TurnAround();
    }

    void TurnAround()
    {
        direction = -direction;
        transform.Rotate(0f, 180f, 0f);
    }
}