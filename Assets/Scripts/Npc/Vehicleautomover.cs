using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class VehicleAutoMover : MonoBehaviour
{
    [Header("Điểm đích")]
    public Transform destination;

    [Header("Thông số di chuyển")]
    public float moveSpeed = 5f;
    public bool rotateTowardsDestination = false;
    public float rotateSpeed = 5f;
    public float stopDistance = 0.2f;

    [Header("Sự kiện (tuỳ chọn)")]
    public UnityEvent onStartMoving;
    public UnityEvent onStopped;
    public UnityEvent onArrived;

    private bool canMove = false;
    private bool hasArrived = false;
    private Rigidbody rb;
    public Vector3 centerOfMassOffset = new Vector3(0, -1f, 0);

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += centerOfMassOffset;
    }

    public void StartMoving()
    {
        if (canMove || hasArrived) return;

        canMove = true;
        Debug.Log($"[VehicleAutoMover] {name} bắt đầu di chuyển tới điểm đích.");
        onStartMoving?.Invoke();
    }

    public void StopMoving()
    {
        if (!canMove) return;

        canMove = false;
        StopHorizontal();
        Debug.Log($"[VehicleAutoMover] {name} dừng lại vì đường bị chặn trở lại.");
        onStopped?.Invoke();
    }

    void Update()
    {
        if (!canMove || hasArrived || destination == null) return;

        Vector3 dest = destination.position;

        Vector3 moveDir = dest - transform.position;
        moveDir.y = 0f;
        moveDir.Normalize();
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);

        if (rotateTowardsDestination)
        {
            Vector3 dir = dest - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
            }
        }

        if (Vector3.Distance(transform.position, dest) <= stopDistance)
        {
            hasArrived = true;
            canMove = false;
            StopHorizontal();
            Debug.Log($"[VehicleAutoMover] {name} đã đến điểm đích.");
            onArrived?.Invoke();
        }
    }

    void StopHorizontal()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }
}