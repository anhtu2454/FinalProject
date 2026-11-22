using UnityEngine;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour
{
    public int angle = 0;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Rotate);
    }

    void Rotate()
    {
        angle = angle + 90;

        if (angle == 360)
        {
            angle = 0;
        }

        ShowAngle();
    }

    public void RandomAngle()
    {
        angle = Random.Range(0, 4) * 90;
        ShowAngle();
    }

    void ShowAngle()
    {
        transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}