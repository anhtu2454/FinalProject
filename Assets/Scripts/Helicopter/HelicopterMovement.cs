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

    [Header("--- Hiệu ứng ---")]
    [SerializeField] private GameObject explosionEffect;

    [Header("--- Nhiên liệu ---")]
    [SerializeField] private float maxFuel = 240f; // Tổng số giây bay được
    private float currentFuel;                     // Nhiên liệu còn lại
    private bool outOfFuel = false;                // true = đã hết xăng

    // Dữ liệu UI đọc
    public float CurrentFuel => currentFuel;
    public float MaxFuel => maxFuel;

    private bool isDead = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentFuel = maxFuel;
    }

    void Update()
    {
        if (isDead) return;

        HandleFuel();

        // Hết xăng thì mất điều khiển: bỏ qua các hàm bay bên dưới
        if (outOfFuel) return;

        HandleLift();
        HandleMovement();
        HandleTiltAndFlip();
    }

    // Mỗi giây trừ 1 đơn vị nhiên liệu
    void HandleFuel()
    {
        if (outOfFuel) return;

        currentFuel -= Time.deltaTime;

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            outOfFuel = true;
            rb.useGravity = true;
        }
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

        Vector3 v = rb.linearVelocity;
        v.y = currentLiftSpeed;
        v.z = 0f;
        rb.linearVelocity = v;
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

        Vector3 v = rb.linearVelocity;
        v.x = currentMoveSpeed;
        v.z = 0f;
        rb.linearVelocity = v;
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

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Victim"))
        {
            return;
        }
        if (other.CompareTag("SwitchZone"))
        {
            return;
        }

        Crash();
    }

    // Hết xăng và rơi trúng vật gì đó thì nổ
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (isDead) return;

    //    if (outOfFuel)
    //    {
    //        Crash();
    //    }
    //}

    private void Crash()
    {
        isDead = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        gameObject.SetActive(false);
        GameManager.Instance.FailMission();
        Debug.Log("Game Over!");
    }
}