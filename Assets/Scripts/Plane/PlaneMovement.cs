using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneMovement : MonoBehaviour
{
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 6f;

    [SerializeField] private float pitchSpeed = 60f;

    [SerializeField] private float fallSpeed = 2.5f; // tốc độ rơi khi nhả ga

    [SerializeField] private float wobbleSpeed = 15f;
    [SerializeField] private float wobbleAngle = 15f;

    [Header("--- Auto Loop khi ngửa/úp quá 90 độ ---")]
    private bool isLooping = false;
    private float loopRemaining = 0f;

    [Header("--- Raycast kiểm tra đường băng ---")]
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private float runwayOffset = 0.1f; // độ cao đặt máy bay so với mặt runway
    [SerializeField] private LayerMask groundLayer;

    [Header("--- Lấy nước ---")]
    [SerializeField] private int maxWaterCapacity = 5;
    [SerializeField] private float collectInterval = 1f;

    private int currentWaterCount = 0;
    private float waterCollectTimer = 0f;

    [Header("--- Hiển thị bình nước trên máy bay ---")]
    [SerializeField] private GameObject waterIconVisual; // object con bình nước

    [Header("--- Ném bình nước ---")]
    [SerializeField] private GameObject waterProjectilePrefab;
    [SerializeField] private float launchSpeed = 8f;
    [SerializeField] private float launchUpwardAngle = 30f; // độ chếch 

    [Header("--- Nhiên liệu (đếm ngược thời gian chơi) ---")]
    [SerializeField] private float maxFuel = 240f; // tổng số giây được chơi

    private float currentFuel;

    [Header("--- Hiệu ứng ---")]
    [SerializeField] private GameObject explosionEffect;

    private const string TagWater = "Water";
    private const string TagRunway = "Runway";

    private bool isDead = false;
    private float currentSpeed;
    private bool isOnRunway = false;

    private Collider[] planeColliders;

    public int CurrentWaterCount => currentWaterCount;
    public int MaxWaterCapacity => maxWaterCapacity;
    public float CurrentFuel => currentFuel;
    public float MaxFuel => maxFuel;

    private void Start()
    {
        currentSpeed = minSpeed;
        currentFuel = maxFuel;

        UpdateWaterIconVisual();

        planeColliders = GetComponentsInChildren<Collider>(includeInactive: true);

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (isDead) return;

        UpdateRunwayStatus();
        HandleFuel();

        HandleThrottle();
        HandlePitch2D();
        HandleWobble();


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isLooping)
            {
                TryLaunchWaterBackward();
            }
            else
            {
                TryDropWaterNormal();
            }
        }

        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
    }

    // Đếm ngược thời gian chơi. Hết nhiên liệu -> dừng trò chơi, không cần nạp lại.
    private void HandleFuel()
    {
        if (currentFuel <= 0f) return;

        currentFuel -= Time.deltaTime;

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            OutOfFuel();
        }
    }

    private void OutOfFuel()
    {
        isDead = true;
        Debug.Log("Hết nhiên liệu! Game Over.");
        Time.timeScale = 0f; // dừng hẳn trò chơi
    }

    // Bắn raycast xuống dưới mỗi frame để xác định có đang ở trên đường băng không
    private void UpdateRunwayStatus()
    {
        Vector3 rayOrigin = transform.position;
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, Color.red);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            isOnRunway = CheckIsRunway(hit.collider.gameObject);

            if (isOnRunway)
            {
                float minY = hit.point.y + runwayOffset;
                if (transform.position.y < minY)
                {
                    Vector3 pos = transform.position;
                    pos.y = minY;
                    transform.position = pos;
                }
            }
        }
        else
        {
            isOnRunway = false;
        }
    }

    //tăng tốc
    private void HandleThrottle()
    {
        bool isThrottling = Input.GetKey(KeyCode.Space);
        float targetSpeed;

        if (isThrottling)
        {
            targetSpeed = maxSpeed;
        }
        else
        {
            targetSpeed = minSpeed;
        }

        if (!isThrottling && !isOnRunway)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    //điều khiển 
    private void HandlePitch2D()
    {
        if (isLooping)
        {
            PerformLoop();
            return;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward * pitchSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * -pitchSpeed * Time.deltaTime);
        }

        float z = GetSignedZAngle();

        // Ngửa lên quá 90 độ
        if (z <= -90f)
        {
            isLooping = true;
            loopRemaining = 270f;
        }

        // Không cho cúi đầu xuống quá 90 độ
        if (z >= 90f)
        {
            Vector3 rot = transform.localEulerAngles;
            rot.z = 90f;
            transform.localEulerAngles = rot;
        }
    }
    private void PerformLoop()
    {
        float step = 270f * Time.deltaTime;

        if (step >= loopRemaining)
        {
            step = loopRemaining;
            isLooping = false;
        }

        transform.Rotate(Vector3.forward * -1f * step);
        loopRemaining -= step;
    }

    // Convert 0-360 sang -180-180
    private float GetSignedZAngle()
    {
        float z = transform.localEulerAngles.z;

        if (z > 180f)
        {
            z = z - 360f;
        }
        return z;
    }


    private void SpawnWaterProjectile(Vector3 velocity)
    {
        GameObject projectile = Instantiate(waterProjectilePrefab, transform.position, Quaternion.identity);

        Collider[] projectileColliders = projectile.GetComponentsInChildren<Collider>(includeInactive: true);
        if (projectileColliders != null && planeColliders != null)
        {
            foreach (Collider pc in projectileColliders)
            {
                if (pc == null) continue;
                foreach (Collider c in planeColliders)
                {
                    if (c != null)
                    {
                        Physics.IgnoreCollision(pc, c, true);
                    }
                }
            }
        }

        WaterProjectile projectileScript = projectile.GetComponent<WaterProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Launch(velocity);
        }
    }


    private void TryDropWaterNormal()
    {
        if (isDead) return;
        if (currentWaterCount <= 0) return;
        if (waterProjectilePrefab == null) return;

        currentWaterCount--;
        UpdateWaterIconVisual();

        // Vận tốc thế giới hiện tại của máy bay: hướng bay (world Vector3.left, xoay theo góc hiện tại) * tốc độ
        Vector3 planeVelocity = transform.TransformDirection(Vector3.left) * currentSpeed;
        SpawnWaterProjectile(planeVelocity);

        Debug.Log($"Thả nước! Còn lại: {currentWaterCount}/{maxWaterCapacity}");
    }


    private void TryLaunchWaterBackward()
    {
        if (isDead) return;
        if (currentWaterCount <= 0) return;
        if (waterProjectilePrefab == null) return;

        currentWaterCount--;
        UpdateWaterIconVisual();

        // Hướng bay chính là world Vector3.left -> bắn ngược lại là world Vector3.right,
        // chếch thêm 1 góc lên để tạo vòng cung rơi xuống tự nhiên khi có trọng lực.
        Vector3 launchDir = Quaternion.Euler(0f, 0f, launchUpwardAngle) * Vector3.right;
        Vector3 velocity = launchDir.normalized * launchSpeed;
        SpawnWaterProjectile(velocity);

        Debug.Log($"Ném nước! Còn lại: {currentWaterCount}/{maxWaterCapacity}");
    }


    private void HandleWobble()
    {
        if (isLooping) return;

        bool isThrottling = Input.GetKey(KeyCode.Space);
        float targetX = 0f;

        if (!isThrottling && !isOnRunway)
        {
            targetX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAngle;
        }

        Vector3 currentRot = transform.localEulerAngles;
        float newX = Mathf.LerpAngle(currentRot.x, targetX, Time.deltaTime * 5f);
        transform.localEulerAngles = new Vector3(newX, currentRot.y, currentRot.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.GetComponent<WaterProjectile>() != null) return;

        if (IsMatchTag(other.gameObject, TagWater))
        {
            Debug.Log("Safe! Water");
            return;
        }

        Crash();
    }


    private void OnTriggerStay(Collider other)
    {
        if (isDead) return;
        if (!IsMatchTag(other.gameObject, TagWater)) return;

        waterCollectTimer += Time.deltaTime;

        if (waterCollectTimer >= collectInterval)
        {
            waterCollectTimer -= collectInterval; // trừ đi thay vì reset về 0
            CollectWater();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsMatchTag(other.gameObject, TagWater))
        {
            waterCollectTimer = 0f;
        }
    }

    private void CollectWater()
    {
        if (currentWaterCount >= maxWaterCapacity) return;

        currentWaterCount++;
        UpdateWaterIconVisual();
        Debug.Log($"Lấy nước: {currentWaterCount}/{maxWaterCapacity}");
    }


    private void UpdateWaterIconVisual()
    {
        if (waterIconVisual != null)
        {
            waterIconVisual.SetActive(currentWaterCount > 0);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.GetComponent<WaterProjectile>() != null) return;

        if (IsMatchTag(collision.gameObject, TagRunway)) return;

        Crash();
    }

    private bool IsMatchTag(GameObject obj, string tag)
    {
        return obj.CompareTag(tag)
            || (obj.transform.parent != null && obj.transform.parent.CompareTag(tag));
    }

    private bool CheckIsRunway(GameObject obj)
    {
        return IsMatchTag(obj, TagRunway);
    }

    private void Crash()
    {
        isDead = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, transform.rotation);
        }

        gameObject.SetActive(false);
        Debug.Log("Game Over!");
    }
}