using UnityEngine;

public class Light : MonoBehaviour
{
    [Tooltip("Tốc độ xoay")]
    public float rotateSpeed = 120f;

    private float currentAngle = 0f;
    private Quaternion initialRotation;

    void Start()
    {

        initialRotation = transform.rotation;
    }

    void Update()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) input = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) input = 1f;

        currentAngle += input * rotateSpeed * Time.deltaTime;

        transform.rotation = initialRotation * Quaternion.Euler(0f, currentAngle, 0f);
    }
}