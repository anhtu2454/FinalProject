using UnityEngine;

public class HookCargoController : MonoBehaviour
{
    [Header("--- Cấu hình dây cẩu ---")]
    [Tooltip("Điểm gắn dây trên thân trực thăng. Kéo vào slot Rope Start / Start Point của asset dây.")]
    public Transform ropeAnchor;

    [Tooltip("Vật móc ở đầu dây, cần có Rigidbody (không kinematic). Kéo vào slot Rope End / End Point của asset dây.")]
    public Transform hookPoint;

    public float minRopeLength = 0f;
    public float maxRopeLength = 6f;

    [Tooltip("Tốc độ thả/thu dây (đơn vị/giây)")]
    public float ropeChangeSpeed = 3f;

    [Header("--- Phím điều khiển ---")]
    public KeyCode extendKey = KeyCode.F;
    public KeyCode retractKey = KeyCode.R;

    // độ dài dây hiện tại
    private float currentRopeLength;
    private Rigidbody hookRigidbody;

    void Start()
    {
        currentRopeLength = minRopeLength;

        if (hookPoint != null)
        {
            hookRigidbody = hookPoint.GetComponent<Rigidbody>();
            if (hookRigidbody == null)
            {
                Debug.LogWarning("HookCargoController: hookPoint cần có Rigidbody để có vật lý quán tính.");
            }
        }
    }

    void Update()
    {
        HandleRopeLength();
    }

    // Xử lý thả/thu dây
    void HandleRopeLength()
    {
        if (Input.GetKey(extendKey))
        {
            currentRopeLength += ropeChangeSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(retractKey))
        {
            currentRopeLength -= ropeChangeSpeed * Time.deltaTime;
        }

        currentRopeLength = Mathf.Clamp(currentRopeLength, minRopeLength, maxRopeLength);
    }

    void FixedUpdate()
    {
        ConstrainHookToRopeLength();
    }

    // Giới hạn dây
    void ConstrainHookToRopeLength()
    {
        if (hookRigidbody == null || ropeAnchor == null) return;

        Vector3 fromAnchorToHook = hookRigidbody.position - ropeAnchor.position;
        float distance = fromAnchorToHook.magnitude;

        if (distance > currentRopeLength && distance > 0.0001f)
        {
            Vector3 direction = fromAnchorToHook / distance;
            Vector3 correctedPosition = ropeAnchor.position + direction * currentRopeLength;
            hookRigidbody.position = correctedPosition;

            Vector3 velocityAlongRope = Vector3.Project(hookRigidbody.linearVelocity, direction);
            hookRigidbody.linearVelocity -= velocityAlongRope;
        }
    }
}