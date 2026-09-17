using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour
{
    [SerializeField] private PlaneMovement plane;
    [SerializeField] private Text waterText;

    private void Update()
    {
        if (plane == null || waterText == null) return;

        waterText.text = $"{plane.CurrentWaterCount}/{plane.MaxWaterCapacity}";
    }
}