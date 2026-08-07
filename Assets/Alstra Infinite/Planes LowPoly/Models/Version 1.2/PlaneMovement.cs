using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
    [Header("--- Cấu hình Tốc độ & Tăng Ga ---")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float acceleration = 6f;      //gia tốc
    public float pitchSpeed = 60f;


    public GameObject explosionEffect;

    private bool isDead = false;
    //tốc độ hiện tại của máy bay
    private float currentSpeed;

    void Start()
    {
        currentSpeed = minSpeed;
    }

    void Update()
    {
        if (isDead) return;
        HandleThrottle();
        HandlePitch2D();

        // lệnh di chuyển
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
    }

    void HandleThrottle()
    {
        // space tăng ga
        bool isThrottling = Input.GetKey(KeyCode.Space);

        float targetSpeed;
        if (isThrottling)
        {
            targetSpeed = maxSpeed;
        } else {
            targetSpeed = minSpeed;
        }
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }


    void HandlePitch2D()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * pitchSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            // Thêm dấu trừ (-) để xoay theo chiều ngược lại
            transform.Rotate(Vector3.forward * -pitchSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        bool isWater = other.CompareTag("Water") || (other.transform.parent != null && other.transform.parent.CompareTag("Water"));

        if (isWater)
        {
            Debug.Log("Safe!");
            return;
        }
        Crash();
    }
    private void Crash()
    {
        isDead = true;

        //if (explosionEffect != null)
        //{
        //    Instantiate(explosionEffect, transform.position, transform.rotation);
        //}

        gameObject.SetActive(false);

        Debug.Log("Game Over!");
    }
}