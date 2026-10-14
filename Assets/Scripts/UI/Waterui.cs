using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour
{
    [SerializeField] private PlaneMovement plane;
    [SerializeField] private Text waterText;

    private void Update()
    {
        waterText.text = $"{plane.CurrentWaterCount}/{plane.MaxWaterCapacity}";
    }
}