using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoadObstacleZone : MonoBehaviour
{
    [Tooltip("Tag của các vật được tính là chướng ngại vật")]
    public string cargoTag = "Cargo";

    [Tooltip("Sự kiện được gọi khi KHÔNG còn vật Cargo nào chạm vùng đường này. " +
             "Kéo xe/tàu vào đây, chọn hàm VehicleAutoMover.StartMoving.")]
    public UnityEvent OnRoadClear;

    [Tooltip("Sự kiện được gọi khi có vật Cargo chạm trở lại sau khi đã thông. " +
             "Kéo xe/tàu vào đây, chọn hàm VehicleAutoMover.StopMoving.")]
    public UnityEvent OnRoadBlocked;

    // Danh sách các collider Cargo hiện đang chạm vùng trigger này
    private HashSet<Collider> obstaclesInZone = new HashSet<Collider>();

    // true = đường đang thông (đã bắn OnRoadClear và chưa bắn lại OnRoadBlocked)
    private bool isClear = true;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(cargoTag)) return;

        bool added = obstaclesInZone.Add(other);
        if (added)
        {
            Debug.Log($"[RoadObstacleZone:{name}] {other.name} đang chặn đường. Tổng: {obstaclesInZone.Count}");
            CheckBlocked();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(cargoTag)) return;

        bool removed = obstaclesInZone.Remove(other);
        if (removed)
        {
            Debug.Log($"[RoadObstacleZone:{name}] {other.name} đã rời khỏi đường. Còn lại: {obstaclesInZone.Count}");
            CheckClear();
        }
    }

    private void CheckClear()
    {
        if (!isClear && obstaclesInZone.Count == 0)
        {
            isClear = true;
            Debug.Log($"[RoadObstacleZone:{name}] Đường đã thông! Gọi OnRoadClear.");
            OnRoadClear?.Invoke();
        }
    }

    private void CheckBlocked()
    {
        if (isClear && obstaclesInZone.Count > 0)
        {
            isClear = false;
            Debug.Log($"[RoadObstacleZone:{name}] Đường bị chặn! Gọi OnRoadBlocked.");
            OnRoadBlocked?.Invoke();
        }
    }
}