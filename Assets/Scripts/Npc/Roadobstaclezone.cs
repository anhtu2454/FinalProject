using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoadObstacleZone : MonoBehaviour
{
    [Tooltip("Các tag vật cản trên đường")]
    public List<string> obstacleTags = new List<string>
    {
        "Cargo",
        "Victim",
        "Fire",
        "Vehicle",
        "VehicleBroken",
        "Helicopter",
        "Water"
    };

    public UnityEvent OnRoadClear;
    public UnityEvent OnRoadBlocked;

    private HashSet<Collider> obstaclesInZone = new HashSet<Collider>();
    private bool isClear = true;

    void Update()
    {
        if (obstaclesInZone.Count == 0) return;

        int removed = obstaclesInZone.RemoveWhere(c =>
            c == null || !c.enabled || !c.gameObject.activeInHierarchy);

        if (removed > 0)
        {
            Debug.Log($"[RoadObstacleZone:{name}] Còn lại: {obstaclesInZone.Count}");
            CheckClear();
        }
    }

    private bool IsObstacle(Collider other)
    {
        for (int i = 0; i < obstacleTags.Count; i++)
        {
            if (other.CompareTag(obstacleTags[i])) return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsObstacle(other)) return;

        if (obstaclesInZone.Add(other))
        {
            Debug.Log($"[RoadObstacleZone:{name}] {other.name} ({other.tag}) đang chặn đường. Tổng: {obstaclesInZone.Count}");
            CheckBlocked();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsObstacle(other)) return;

        if (obstaclesInZone.Remove(other))
        {
            Debug.Log($"[RoadObstacleZone:{name}] {other.name} ({other.tag}) đã rời khỏi đường. Còn lại: {obstaclesInZone.Count}");
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