using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    [SerializeField] private PlaneMovement plane;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private Image fillImage;

    [Header("--- Cảnh báo sắp hết ---")]
    [SerializeField][Range(0f, 1f)] private float lowFuelThreshold = 0.25f; // dưới 25% đổi màu
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowFuelColor = Color.red;

    private void Update()
    {
        if (plane == null || fuelSlider == null) return;
        if (plane.MaxFuel <= 0f) return;

        float fuelPercent = plane.CurrentFuel / plane.MaxFuel;
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