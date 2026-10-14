using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestinationZone : MonoBehaviour
{
    public string vehicleTag = "Vehicle";

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) 
        { 
            col.isTrigger = true; 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(vehicleTag)) return;
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy GameManager trong scene.");
            return;
        }

        GameManager.Instance.RegisterVehicleArrived(other.transform);
    }
}