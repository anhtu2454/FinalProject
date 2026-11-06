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

    [Header("Sound")]
    public AudioClip breakClip;
    [Range(0f, 1f)] public float breakVolume = 1f;
    public float soundMinDistance = 20f;
    public float soundMaxDistance = 200f;

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

        PlayBreakSound(hitPoint);

        foreach (Rigidbody rb in pieces)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, hitPoint, explosionRadius, 0f, ForceMode.Impulse);
        }
    }

    //âm thanh
    void PlayBreakSound(Vector3 position)
    {
        if (breakClip == null) return;

        // object tạm chứa AudioSource, tự xóa sau khi phát xong
        GameObject soundObj = new GameObject("WallBreakSound");
        soundObj.transform.position = position;

        AudioSource src = soundObj.AddComponent<AudioSource>();
        src.clip = breakClip;
        src.volume = breakVolume;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = soundMinDistance;
        src.maxDistance = soundMaxDistance;
        src.Play();

        Destroy(soundObj, breakClip.length + 0.1f);
    }
}