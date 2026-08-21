using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // kéo máy bay (LightPlane) vào đây
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.08f, 5.45f); // khoảng cách camera so với máy bay

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position + offset;

    }
}