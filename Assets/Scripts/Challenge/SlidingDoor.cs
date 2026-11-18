using UnityEngine;


public class SlidingDoor : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveSpeed = 2f;

    private Vector3 targetPosition;
    private bool isOpening = false;

    void Start()
    {
        targetPosition = transform.position + Vector3.down * moveDistance;
    }

    public void Open()
    {
        isOpening = true;
    }

    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }
}