using UnityEngine;

public class RisingGas : MonoBehaviour
{
    [Tooltip("Tốc độ khí lan sang phải (đơn vị/giây)")]
    public float spreadSpeed = 1f;

    [Tooltip("Máu victim bị trừ mỗi giây")]
    public float damagePerSecond = 5f;

    [Tooltip("Tag của victim")]
    public string victimTag = "Victim";

    [Tooltip("Hiệu ứng khí ga")]
    public Transform gasEffect;

    void Update()
    {
        float growth = spreadSpeed * Time.deltaTime;

        Vector3 scale = transform.localScale;
        scale.x += growth;
        transform.localScale = scale;

        // dời khối sang phải nửa đoạn vừa dài thêm để mép trái đứng yên
        transform.position += Vector3.right * (growth / 2f);

        // đưa hiệu ứng tới mép phải của khối
        if (gasEffect != null)
        {
            Vector3 pos = gasEffect.position;
            pos.x = transform.position.x + scale.x / 2f;
            gasEffect.position = pos;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(victimTag)) return;

        VictimHealth victim = other.GetComponent<VictimHealth>();
        if (victim != null)
        {
            victim.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}