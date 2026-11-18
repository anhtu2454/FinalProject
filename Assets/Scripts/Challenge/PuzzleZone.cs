using UnityEngine;


public class PuzzleZone : MonoBehaviour
{
    public GameObject puzzlePanel;
    public Puzzle[] tiles;
    public SlidingDoor door;

    private bool isSolved = false;

    void Start()
    {
        puzzlePanel.SetActive(false);
    }

    // trực thăng bay vào vùng này
    void OnTriggerEnter(Collider other)
    {
        if (isSolved || puzzlePanel.activeSelf) return;

        // chỉ trực thăng mới mở được câu đố
        if (other.GetComponentInParent<HelicopterMovement>() == null) return;

        // xoay ngẫu nhiên 9 ô
        foreach (Puzzle tile in tiles)
        {
            tile.RandomAngle();
        }

        puzzlePanel.SetActive(true);
        Time.timeScale = 0f;   // dừng game
    }

    // mỗi frame kiểm tra: cả 9 ô đã về góc 0 chưa
    void Update()
    {
        if (isSolved || !puzzlePanel.activeSelf) return;

        foreach (Puzzle tile in tiles)
        {
            if (tile.angle != 0) return;
        }

        isSolved = true;
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f;   // chạy game lại
        door.Open();
    }
}