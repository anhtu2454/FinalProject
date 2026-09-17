using UnityEngine;

public class PlaneSwitchZone : MonoBehaviour
{
    [SerializeField] private GameObject currentPlane;
    [SerializeField] private GameObject nextPlane;
    [SerializeField] private GameObject hook;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private GameObject[] uiToHideOnSwitch;

    private bool hasSwitched = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasSwitched) return; 
        if (other.gameObject != currentPlane) return;

        hasSwitched = true;

        currentPlane.SetActive(false);

        nextPlane.SetActive(true);
        hook.SetActive(true);
        PlaneMovement newMovement = nextPlane.GetComponent<PlaneMovement>();
        if (newMovement != null)
        {
            newMovement.enabled = true;
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetTarget(nextPlane.transform);
        }

        if (uiToHideOnSwitch != null)
        {
            foreach (GameObject ui in uiToHideOnSwitch)
            {
                if (ui != null)
                {
                    ui.SetActive(false);
                }
            }
        }

        Debug.Log("Đã chuyển sang điều khiển máy bay tiếp theo!");
    }
}