using UnityEngine;

// CHỈ DÙNG ĐỂ TEST. Xóa hoặc tắt object chứa script này trước khi build game thật.
public class DebugCheat : MonoBehaviour
{
    [Header("Phím tắt")]
    public KeyCode completeKey = KeyCode.F9;
    public KeyCode failKey = KeyCode.F10;

    void Update()
    {
        if (Input.GetKeyDown(completeKey))
        {
            CompleteNow();
        }
        if (Input.GetKeyDown(failKey))
        {
            FailNow();
        }
    }

    public void CompleteNow()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.DebugForceComplete();
    }


    public void FailNow()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.FailMission();
    }
}