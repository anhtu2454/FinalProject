using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public Transform[] planes;

    public GameObject meteorPrefab;

    [Header("Vùng rơi")]
    public float halfWidth = 20f;

    public float spawnHeight = 30f;

    [Header("Thời gian giữa 2 thiên thạch (giây)")]
    public float minInterval = 2f;
    public float maxInterval = 4f;

    private float timer;

    void Start()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        timer = Random.Range(minInterval, maxInterval);

        Transform plane = GetActivePlane();
        if (plane == null || meteorPrefab == null) return;

        Vector3 spawnPosition = plane.position + new Vector3(Random.Range(-halfWidth, halfWidth), spawnHeight, 0f);
        Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
    }

    Transform GetActivePlane()
    {
        foreach (Transform plane in planes)
        {
            if (plane != null && plane.gameObject.activeInHierarchy)
            {
                return plane;
            }
        }
        return null;
    }
}