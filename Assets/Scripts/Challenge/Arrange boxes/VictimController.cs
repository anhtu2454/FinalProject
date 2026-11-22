using UnityEngine;

public class VictimController : MonoBehaviour
{
    public float moveSpeed = 4f;        // Tốc độ đi
    public float jumpSpeed = 4f;
    public Transform holdPoint;         // Chỗ đặt thùng khi bê (object con đặt trước mặt victim)
    public Transform[] boxes;
    public float pickupDistance = 1.5f; // Đứng cách thùng bao xa thì nhấc được

    private Rigidbody rb;
    private Animator animator;
    private Transform carriedBox;     // Thùng đang bê (null = tay không)
    private float moveInput;            // -1 = đi trái, 1 = đi phải, 0 = đứng yên

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1;
        if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1;

        if (moveInput > 0) transform.rotation = Quaternion.Euler(0, 90, 0);
        if (moveInput < 0) transform.rotation = Quaternion.Euler(0, -90, 0);

        bool isOnGround = Mathf.Abs(rb.linearVelocity.y) < 0.1f;

        if (Input.GetKeyDown(KeyCode.UpArrow) && isOnGround)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpSpeed, 0f);
        }

        if (animator != null) animator.SetBool("IsRunning", moveInput != 0);

        //nhặt thả thùng
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (carriedBox == null) PickUpBox();
            else DropBox();
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveInput * moveSpeed, rb.linearVelocity.y, 0);
    }

    void PickUpBox()
    {
        // Tìm thùng gần nhất trong tầm nhấc
        Transform nearest = null;
        float nearestDistance = pickupDistance;

        foreach (Transform box in boxes)
        {
            float distance = Vector3.Distance(transform.position, box.position);
            if (distance < nearestDistance)
            {
                nearest = box;
                nearestDistance = distance;
            }
        }

        if (nearest == null) return; // Không có thùng nào gần

        // Gắn thùng vào holdPoint
        carriedBox = nearest;
        carriedBox.GetComponent<Rigidbody>().isKinematic = true;
        carriedBox.GetComponent<Collider>().enabled = false;
        carriedBox.SetParent(holdPoint);
        carriedBox.localPosition = Vector3.zero;
    }

    void DropBox()
    {
        // Thả thùng xuống, cho rơi theo vật lý
        carriedBox.SetParent(null);
        carriedBox.GetComponent<Rigidbody>().isKinematic = false;
        carriedBox.GetComponent<Collider>().enabled = true;
        carriedBox = null;
    }
}