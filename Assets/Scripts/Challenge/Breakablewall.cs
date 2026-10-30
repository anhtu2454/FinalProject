using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Tooltip("Tag của vật đập vào tường")]
    public string breakerTag = "Cargo";

    [Tooltip("Tốc độ va chạm tối thiểu để tường vỡ")]
    public float breakSpeed = 4f;

    [Header("Mảnh vỡ")]
    [Tooltip("Lực đẩy các mảnh văng ra lúc vỡ")]
    public float explosionForce = 6f;
    public float explosionRadius = 5f;

    private readonly List<Rigidbody> pieces = new List<Rigidbody>();
    private bool broken = false;

    void Awake()
    {
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            GameObject piece = meshRenderer.gameObject;

            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb == null) continue;

            // đóng băng để tường đứng yên
            rb.isKinematic = true;
            pieces.Add(rb);

            WallPiece wallPiece = piece.GetComponent<WallPiece>();
            if (wallPiece == null)
            {
                wallPiece = piece.AddComponent<WallPiece>();
            }
            wallPiece.wall = this;
        }
    }

    // gọi khi có vật va vào
    public void OnPieceHit(Collision collision)
    {
        if (broken) return;
        if (!collision.gameObject.CompareTag(breakerTag)) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < breakSpeed) return;

        Break(collision.GetContact(0).point);
    }

    void Break(Vector3 hitPoint)
    {
        broken = true;

        foreach (Rigidbody rb in pieces)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius, 0f, ForceMode.Impulse);
        }
    }
}