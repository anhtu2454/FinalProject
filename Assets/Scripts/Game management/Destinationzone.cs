using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestinationZone : MonoBehaviour
{
    public string vehicleTag = "Vehicle";
    public string brokenVehicleTag = "VehicleBroken";

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    bool IsVehicle(Collider other)
    {
        return other.CompareTag(vehicleTag) || other.CompareTag(brokenVehicleTag);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsVehicle(other)) return;
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy GameManager trong scene.");
            return;
        }

        GameManager.Instance.RegisterVehicleArrived(other.transform);
    }


    void OnTriggerExit(Collider other)
    {
        if (!IsVehicle(other)) return;
        if (GameManager.Instance == null) return;

        GameManager.Instance.UnregisterVehicle(other.transform);
    }
}