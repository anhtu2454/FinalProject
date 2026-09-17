using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    private bool hasOffset = false;

    private void Start()
    {
        if (target != null && !hasOffset)
        {
            offset = transform.position - target.position;
            hasOffset = true;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + offset;
    }
}