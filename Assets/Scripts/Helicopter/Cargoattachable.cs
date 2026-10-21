using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CargoAttachable : MonoBehaviour
{
    [Header("Tag cargo")]
    public string cargoTag = "Cargo";
    public string victimTag = "Victim";
    public string brokenVehicleTag = "VehicleBroken";

    [Tooltip("Phím móc hàng khi đang chạm vào Cargo")]
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
        if (!(collision.gameObject.CompareTag(cargoTag) || collision.gameObject.CompareTag(victimTag) || collision.gameObject.CompareTag(brokenVehicleTag))) return;

        nearbyCargo = collision.rigidbody != null ? collision.rigidbody : collision.gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionExit(Collision collision)
    {
        if (!(collision.gameObject.CompareTag(cargoTag) || collision.gameObject.CompareTag(victimTag) || collision.gameObject.CompareTag(brokenVehicleTag))) return;

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

        VictimAnimation victim = cargoRb.GetComponent<VictimAnimation>();
        if (victim != null) victim.OnHooked();
    }

    public void ReleaseCargo()
    {
        if (currentCargo != null)
        {
            VictimAnimation victim = currentCargo.GetComponent<VictimAnimation>();
            if (victim != null) victim.OnReleased();
        }

        if (currentJoint != null)
        {
            Destroy(currentJoint);
        }

        currentJoint = null;
        currentCargo = null;
    }
}