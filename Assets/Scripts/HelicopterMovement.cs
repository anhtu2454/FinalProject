using UnityEngine;

public class HelicopterMovement : MonoBehaviour
{
    [Header("--- Cấu hình tốc độ trực thăng ---")]
    public float maxMoveSpeed = 5f;

    public float maxLiftSpeed = 3f;

    public float acceleration = 4f;

    [Header("--- Cấu hình nghiêng và quay đầu ---")]
    //góc nghiêng máy bay
    public float maxTiltAngle = 15f;

    [Tooltip("Tốc độ xoay đầu và nghiêng")]
    public float tiltSpeed = 5f;

    // biến lưu vận tốc di chuyển thực tế hiện tại
    private float currentMoveSpeed = 0f;
    private float currentLiftSpeed = 0f;

    // biến lưu góc quay mặt hiện tại (0 độ hoặc 180 độ)
    private float currentFacingY = 0f;

    void Update()
    {
        HandleLift();
        HandleMovement();
        HandleTiltAndFlip();
    }


    //lên / xuống
    void HandleLift()
    {
        float targetLift = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            targetLift = maxLiftSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetLift = -maxLiftSpeed;
        }
        
        currentLiftSpeed = Mathf.MoveTowards(currentLiftSpeed, targetLift, acceleration * Time.deltaTime);
        transform.Translate(Vector3.up * currentLiftSpeed * Time.deltaTime, Space.World);
    }

    //tiến / lùi
    void HandleMovement()
    {
        float targetMove = 0f;

        if (Input.GetKey(KeyCode.D))
        {
            targetMove = maxMoveSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            targetMove = -maxMoveSpeed;
        }

        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, targetMove, acceleration * Time.deltaTime);
        transform.Translate(Vector3.right * currentMoveSpeed * Time.deltaTime, Space.World);
    }


    //quay mặt 180 độ và chúi đầu
    void HandleTiltAndFlip()
    {
        if (currentMoveSpeed > 0.1f)
        {
            currentFacingY = 0f;
        }
        else if (currentMoveSpeed < -0.1f)
        {
            currentFacingY = 180f;
        }


        float absoluteSpeedRatio = Mathf.Abs(currentMoveSpeed) / maxMoveSpeed;


        float targetTiltAngle = -absoluteSpeedRatio * maxTiltAngle;

        Quaternion targetRotation = Quaternion.Euler(0, currentFacingY, targetTiltAngle);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, tiltSpeed * Time.deltaTime);
    }
}