using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] private PlaneMovement plane;
    [SerializeField] private HelicopterMovement helicopter;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Image fillImage;

    [Header("--- Cảnh báo sắp hết ---")]
    [SerializeField][Range(0f, 1f)] private float lowFuelThreshold = 0.25f; // dưới 25% đổi màu
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowFuelColor = Color.red;

    private void Update()
    {
        if (fuelSlider == null) return;

        // Xem đang lái máy bay hay trực thăng (cái nào đang bật thì lấy xăng của cái đó)
        float currentFuel;
        float maxFuel;

        if (plane != null && plane.gameObject.activeInHierarchy)
        {
            currentFuel = plane.CurrentFuel;
            maxFuel = plane.MaxFuel;
        }
        else if (helicopter != null && helicopter.gameObject.activeInHierarchy)
        {
            currentFuel = helicopter.CurrentFuel;
            maxFuel = helicopter.MaxFuel;
        }
        else
        {
            return; // Không có cái nào đang bật
        }

        if (maxFuel <= 0f) return;

        float fuelPercent = currentFuel / maxFuel;
        fuelSlider.value = fuelPercent;

        if (fillImage != null)
        {
            if (fuelPercent <= lowFuelThreshold)
            {
                fillImage.color = lowFuelColor;
            }
            else
            {
                fillImage.color = normalColor;
            }
        }
    }
}