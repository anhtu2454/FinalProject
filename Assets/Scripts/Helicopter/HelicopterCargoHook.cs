using UnityEngine;

/// <summary>
/// Gắn script này vào chính GameObject trực thăng (hoặc 1 object quản lý riêng).
/// Cần chuẩn bị trong scene:
///  - AnchorPoint: Empty GameObject làm CON của trực thăng (đáy thân máy bay),
///    có Rigidbody (Is Kinematic = true).
///  - HookObject: GameObject KHÔNG làm con của trực thăng, có Rigidbody (không kinematic)
///    + Collider (để va chạm núi/địa hình thật) + SpringJoint
///    (Connected Body = Rigidbody của AnchorPoint, Max Distance = ropeLength).
///  - Việc vẽ dây (đẹp, có độ võng) nên dùng asset "Optimized Ropes And Cables Tool":
///    gán Rope Start = AnchorPoint, Rope End = HookObject trong component Rope của asset đó.
///    ropeRenderer / ropeTube dưới đây chỉ là phương án dự phòng nếu không dùng asset.
/// </summary>
public class HelicopterCargoHook : MonoBehaviour
{
    [Header("--- Tham chiếu ---")]
    public Transform anchorPoint;
    public Transform hookTransform;

    [Header("--- Vẽ dây dự phòng (bỏ trống nếu dùng asset Optimized Ropes And Cables) ---")]
    public LineRenderer ropeRenderer;
    public Transform ropeTube;
    public float ropeThickness = 0.05f;

    [Header("--- Cấu hình chiều dài dây ---")]
    public float ropeLength = 4f;
    public float minRopeLength = 1f;
    public float maxRopeLength = 8f;
    public float winchSpeed = 2f;
    public KeyCode winchUpKey = KeyCode.R;   // thu dây lại (kéo đồ lên)
    public KeyCode winchDownKey = KeyCode.F; // nhả dây ra (thả đồ xuống)

    [Header("--- Vật lý dây (SpringJoint) ---")]
    [Tooltip("Rigidbody (Is Kinematic = true) gắn trên chính AnchorPoint")]
    public Rigidbody anchorRigidbody;
    [Tooltip("Rigidbody (không kinematic) + Collider gắn trên HookObject")]
    public Rigidbody hookRigidbody;
    [Tooltip("SpringJoint gắn trên HookObject, Connected Body = anchorRigidbody")]
    public SpringJoint ropeJoint;

    [Header("--- Thu/nhả theo đốt dây (dùng khi dây là Rope-Generator, nhiều đốt) ---")]
    [Tooltip("Danh sách Rigidbody các đốt dây, xếp ĐÚNG THỨ TỰ từ đốt gần AnchorDriver (đầu tiên) tới đốt gần móc (cuối cùng). Bỏ trống nếu không dùng tính năng này.")]
    public Rigidbody[] orderedSegments;
    [Tooltip("Số đốt tối thiểu luôn để tự do đung đưa, không thu hết được -- tránh móc bị kéo dính sát gầm máy bay.")]
    public int minActiveSegments = 2;
    [Tooltip("Thời gian (giây) giữa mỗi lần thu/nhả 1 đốt -- càng nhỏ thu/nhả càng nhanh.")]
    public float winchStepInterval = 0.15f;

    private int activeSegmentCount;
    private float winchTimer;

    [Header("--- Cấu hình móc/thả đồ ---")]
    public KeyCode grabKey = KeyCode.E;
    public float grabRadius = 0.5f;
    public LayerMask cargoLayer;
    [Tooltip("Collider của các đốt dây gần móc (thường 1-2 đốt cuối) -- vật đang cẩu sẽ được bỏ va chạm với đúng những collider này, tránh đè lên nhau gây giật.")]
    public Collider[] hookColliders;

    private Transform carriedObject;
    private Rigidbody carriedRb;
    private Collider carriedCollider;
    private Vector3 carryLocalOffset; // vị trí tương đối của vật so với móc, tính trong không gian local của hookTransform, ghi nhận lúc nhặt

    void Start()
    {
        // nếu chưa đặt vị trí ban đầu cho móc thì đặt thẳng dưới anchor
        if (hookTransform.position == Vector3.zero)
        {
            hookTransform.position = anchorPoint.position + Vector3.down * ropeLength;
        }

        if (ropeJoint != null)
        {
            ropeJoint.maxDistance = ropeLength;
        }

        // ban đầu toàn bộ đốt dây đều "đang thả" (tự do đung đưa)
        activeSegmentCount = orderedSegments != null ? orderedSegments.Length : 0;
    }

    void FixedUpdate()
    {
        // các đốt đã bị "thu" (kinematic) luôn bám sát đúng vị trí đốt active cuối cùng,
        // để trông như cả cụm bị cuộn gọn lại 1 chỗ thay vì đứng yên tại chỗ cũ
        if (orderedSegments == null || orderedSegments.Length == 0) return;
        if (activeSegmentCount >= orderedSegments.Length) return;

        var followTarget = orderedSegments[Mathf.Max(activeSegmentCount - 1, 0)];
        if (followTarget == null) return;

        for (int i = activeSegmentCount; i < orderedSegments.Length; i++)
        {
            var seg = orderedSegments[i];
            if (seg == null) continue;
            seg.MovePosition(followTarget.transform.position);
            seg.MoveRotation(followTarget.transform.rotation);
        }
    }

    void Update()
    {
        HandleWinch();
        DrawRope(); // chỉ có tác dụng nếu ropeRenderer/ropeTube được gán
        HandleGrabInput();

        // vật đang cẩu bám theo móc, giữ đúng khoảng lệch đã ghi nhận lúc nhặt lên
        if (carriedObject != null)
        {
            carriedObject.position = hookTransform.TransformPoint(carryLocalOffset);
        }
    }

    // thu / nhả dây bằng phím -> điều khiển giới hạn khoảng cách của SpringJoint
    // (chỉ có tác dụng nếu bạn dùng phương án SpringJoint, không dùng Rope-Generator)
    void HandleWinch()
    {
        if (Input.GetKey(winchUpKey))
        {
            ropeLength -= winchSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(winchDownKey))
        {
            ropeLength += winchSpeed * Time.deltaTime;
        }

        ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);

        if (ropeJoint != null)
        {
            ropeJoint.maxDistance = ropeLength;
        }

        // thu/nhả theo đốt dây, dùng khi orderedSegments được gán (Rope-Generator)
        HandleSegmentWinch();
    }

    void HandleSegmentWinch()
    {
        if (orderedSegments == null || orderedSegments.Length == 0) return;

        winchTimer -= Time.deltaTime;
        if (winchTimer > 0f) return;

        if (Input.GetKey(winchUpKey) && activeSegmentCount > minActiveSegments)
        {
            RetractOneSegment();
            winchTimer = winchStepInterval;
        }
        else if (Input.GetKey(winchDownKey) && activeSegmentCount < orderedSegments.Length)
        {
            ExtendOneSegment();
            winchTimer = winchStepInterval;
        }
    }

    // "Thu" -- đốt gần móc nhất hiện đang tự do sẽ bị đóng băng (kinematic), cuộn về sát đốt trước nó
    void RetractOneSegment()
    {
        activeSegmentCount--;
        var seg = orderedSegments[activeSegmentCount];
        if (seg == null) return;

        seg.linearVelocity = Vector3.zero;
        seg.angularVelocity = Vector3.zero;
        seg.isKinematic = true;

        var col = seg.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    // "Nhả" -- trả vật lý cho đốt kế tiếp, để nó rơi/đung đưa tự do trở lại
    void ExtendOneSegment()
    {
        var seg = orderedSegments[activeSegmentCount];
        if (seg != null)
        {
            var col = seg.GetComponent<Collider>();
            if (col != null) col.enabled = true;
            seg.isKinematic = false;
        }

        activeSegmentCount++;
    }

    // vẽ dây thủ công (LineRenderer phẳng hoặc Cylinder 3D) -- bỏ qua nếu dùng asset Rope
    void DrawRope()
    {
        if (ropeRenderer != null)
        {
            ropeRenderer.positionCount = 2;
            ropeRenderer.SetPosition(0, anchorPoint.position);
            ropeRenderer.SetPosition(1, hookTransform.position);
        }

        if (ropeTube != null)
        {
            Vector3 dir = hookTransform.position - anchorPoint.position;
            float dist = dir.magnitude;

            ropeTube.position = anchorPoint.position + dir * 0.5f;
            if (dir != Vector3.zero)
            {
                ropeTube.up = dir.normalized;
            }

            Vector3 scale = ropeTube.localScale;
            scale.y = dist / 2f; // cylinder mặc định cao 2 đơn vị
            scale.x = ropeThickness;
            scale.z = ropeThickness;
            ropeTube.localScale = scale;
        }
    }

    void HandleGrabInput()
    {
        if (!Input.GetKeyDown(grabKey)) return;

        if (carriedObject == null)
        {
            TryGrabObject();
        }
        else
        {
            ReleaseObject();
        }
    }

    void TryGrabObject()
    {
        Collider[] hits = Physics.OverlapSphere(hookTransform.position, grabRadius, cargoLayer);
        if (hits.Length == 0) return;

        Collider hit = hits[0];
        carriedObject = hit.transform;
        carriedCollider = hit;
        carriedRb = hit.GetComponent<Rigidbody>();

        // Ghi nhận vị trí tương đối của vật so với móc ngay lúc nhặt (trong không gian local
        // của hookTransform) -- giữ nguyên khoảng lệch này trong suốt quá trình cẩu, tránh vật
        // bị "nhảy" về đúng tâm móc (gây hiện tượng tụt lên/lệch khỏi móc nếu pivot vật không ở tâm).
        carryLocalOffset = hookTransform.InverseTransformPoint(carriedObject.position);

        if (carriedRb != null)
        {
            carriedRb.linearVelocity = Vector3.zero;
            carriedRb.angularVelocity = Vector3.zero;
            carriedRb.isKinematic = true; // tắt vật lý trong lúc bị cẩu
        }

        // Bỏ va chạm giữa vật đang cẩu và các đốt dây gần móc -- tránh 2 collider đè lên
        // nhau gây giật khi vật bị ép về đúng vị trí hookTransform mỗi frame.
        SetIgnoreHookCollisions(true);
    }

    void ReleaseObject()
    {
        if (carriedRb != null)
        {
            carriedRb.isKinematic = false; // trả vật lý lại khi thả rơi
        }

        // Bật lại va chạm với các đốt dây gần móc trước khi bỏ tham chiếu
        SetIgnoreHookCollisions(false);

        carriedObject = null;
        carriedRb = null;
        carriedCollider = null;
    }

    // Bật/tắt va chạm giữa collider của vật đang cẩu và các đốt dây gần móc (hookColliders).
    void SetIgnoreHookCollisions(bool ignore)
    {
        if (carriedCollider == null) return;

        foreach (var hookCollider in hookColliders)
        {
            if (hookCollider == null) continue;
            Physics.IgnoreCollision(carriedCollider, hookCollider, ignore);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hookTransform == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hookTransform.position, grabRadius);
    }
}