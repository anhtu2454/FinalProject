using UnityEngine;
using UnityEngine.UI;


public class BoxPuzzle : MonoBehaviour
{
    public Transform[] boxes;   // 5 thùng
    public Transform[] slots;    // 5 vị trí đúng
    public float tolerance = 0.5f; // Thùng lệch vị trí 
    public SlidingDoor door;
    public Text resultText;

    private bool solved = false;

    void Start()
    {
        resultText.gameObject.SetActive(false);
    }

    // Victim đi vào vùng nút thì kiểm tra
    void OnTriggerEnter(Collider other)
    {
        if (solved) return;
        if (other.GetComponentInParent<VictimController>() == null) return; // Chỉ victim mới bấm được

        CheckBoxes();
    }

    void CheckBoxes()
    {
        // Đếm số thùng đang nằm đúng chỗ
        int correct = 0;
        for (int i = 0; i < boxes.Length; i++)
        {
            float distance = Vector2.Distance(boxes[i].position, slots[i].position);
            if (distance < tolerance) correct++;
        }

        // Hiện kết quả, ví dụ 3/5
        resultText.text = correct + ":" + boxes.Length;
        resultText.gameObject.SetActive(true);
        CancelInvoke("HideText");

        if (correct == boxes.Length)
        {
            solved = true;
            door.Open();
        }
        else
        {
            Invoke("HideText", 2f); // Chưa đúng thì 2 giây sau ẩn chữ
        }
    }

    void HideText()
    {
        resultText.gameObject.SetActive(false);
    }
}