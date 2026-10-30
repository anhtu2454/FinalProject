using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoadObstacleZone : MonoBehaviour
{
    [Tooltip("Tag cargo")]
    public string cargoTag = "Cargo";

    public UnityEvent OnRoadClear;

    public UnityEvent OnRoadBlocked;

    private HashSet<Collider> obstaclesInZone = new HashSet<Collider>();

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
            Debug.Log($"[RoadObstacleZone:{name}] Đường đã thông");
            OnRoadClear?.Invoke();
        }
    }

    private void CheckBlocked()
    {
        if (isClear && obstaclesInZone.Count > 0)
        {
            isClear = false;
            Debug.Log($"[RoadObstacleZone:{name}] Đường bị chặn");
            OnRoadBlocked?.Invoke();
        }
    }
}