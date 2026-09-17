using UnityEngine;
using UnityEngine.Events;

public class VehicleAutoMover : MonoBehaviour
{
    [Header("Điểm đích")]
    public Transform destination;

    [Header("Thông số di chuyển")]
    public float moveSpeed = 5f;
    [Tooltip("Phương tiện có tự xoay mặt theo hướng di chuyển không.")]
    public bool rotateTowardsDestination = true;
    public float rotateSpeed = 5f;
    [Tooltip("Khoảng cách nhỏ hơn giá trị này coi như đã đến nơi.")]
    public float stopDistance = 0.2f;

    [Header("Sự kiện (tuỳ chọn)")]
    public UnityEvent onStartMoving;
    public UnityEvent onStopped;
    public UnityEvent onArrived;

    private bool canMove = false;
    private bool hasArrived = false;


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
        Debug.Log($"[VehicleAutoMover] {name} dừng lại vì đường bị chặn trở lại.");
        onStopped?.Invoke();
    }

    void Update()
    {
        if (!canMove || hasArrived || destination == null) return;

        Vector3 dest = destination.position;
        transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.deltaTime);

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
            Debug.Log($"[VehicleAutoMover] {name} đã đến điểm đích.");
            onArrived?.Invoke();
        }
    }
}