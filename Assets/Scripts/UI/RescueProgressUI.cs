using UnityEngine;
using UnityEngine.UI;

public class RescueProgressUI : MonoBehaviour
{
    [SerializeField] private Text fireText;
    [SerializeField] private Text victimText;
    [SerializeField] private Text vehicleText;

    void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }
        fireText.text = $"{GameManager.Instance.RemainingFires}";
        victimText.text = $"{GameManager.Instance.RemainingVictims}";
        vehicleText.text = $"{GameManager.Instance.RemainingVehicles}";
    }
}
