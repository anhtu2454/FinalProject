using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CargoAttachable : MonoBehaviour
{
    [Header("--- Cấu hình gắn hàng cẩu ---")]
    public string cargoTag = "Cargo";

    [Tooltip("Phím chủ động móc hàng khi đang chạm vào Cargo")]
    public KeyCode attachKey = KeyCode.E;

    [Tooltip("Phím thả hàng đang cẩu xuống")]
    public KeyCode releaseKey = KeyCode.E;

    private FixedJoint currentJoint;
    private Rigidbody currentCargo;

    private Rigidbody nearbyCargo;

    void Update()
    {
        if (currentCargo == null && nearbyCargo != null && Input.GetKeyDown(attachKey))
        {
            AttachCargo(nearbyCargo);
        }
        else if (currentCargo != null && Input.GetKeyDown(releaseKey))
        {
            ReleaseCargo();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentCargo != null) return; 
        if (!collision.gameObject.CompareTag(cargoTag)) return;

        nearbyCargo = collision.rigidbody != null ? collision.rigidbody : collision.gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag(cargoTag)) return;

        Rigidbody exitingRb = collision.rigidbody != null ? collision.rigidbody : collision.gameObject.GetComponent<Rigidbody>();
        if (exitingRb == nearbyCargo)
        {
            nearbyCargo = null;
        }
    }

    void AttachCargo(Rigidbody cargoRb)
    {
        if (cargoRb == null) return;

        currentCargo = cargoRb;
        nearbyCargo = null;

        currentJoint = gameObject.AddComponent<FixedJoint>();
        currentJoint.connectedBody = cargoRb;
        currentJoint.breakForce = Mathf.Infinity;
        currentJoint.breakTorque = Mathf.Infinity;
    }

    void ReleaseCargo()
    {
        if (currentJoint != null)
        {
            Destroy(currentJoint);
        }

        currentJoint = null;
        currentCargo = null;
    }
}