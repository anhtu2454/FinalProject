using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("kiểm tra lửa")]
    public string fireTag = "Fire";
    [Tooltip("khoảng cách kiểm tra")]
    public float fireCheckInterval = 0.5f;

    [Header("kiểm tra xe")]
    public string vehicleTag = "Vehicle";

    [Header("Sự kiện khi hoàn thành màn")]
    public UnityEvent LevelComplete;
    [Header("Sự kiện khi thất bại")]
    public UnityEvent LevelFail;

    private int totalVehicles;
    private int totalFires;
    private int remainingFires;
    private readonly HashSet<Transform> arrivedVehicles = new HashSet<Transform>();
    private bool levelCompleted = false;
    private float fireCheckTimer;

    // Số liệu để UI đọc
    public int TotalFires => totalFires;
    public int RemainingFires => remainingFires;
    public int TotalVehicles => totalVehicles;
    public int ArrivedVehicles => arrivedVehicles.Count;
    public int RemainingVehicles => Mathf.Max(totalVehicles - arrivedVehicles.Count, 0);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        totalVehicles = GameObject.FindGameObjectsWithTag(vehicleTag).Length;
        totalFires = GameObject.FindGameObjectsWithTag(fireTag).Length;
        remainingFires = totalFires;

        if (totalVehicles == 0)
        {
            Debug.LogWarning($"GameManager: Không tìm thấy xe nào với tag '{vehicleTag}'. Kiểm tra lại tag đã gán trên các xe cần cứu chưa.");
        }
        if (totalFires == 0)
        {
            Debug.LogWarning($"GameManager: Không tìm thấy đám lửa nào với tag '{fireTag}'. Kiểm tra lại tag đã gán trên các đám lửa chưa.");
        }
    }

    void Update()
    {
        if (levelCompleted) return;

        fireCheckTimer -= Time.deltaTime;
        if (fireCheckTimer <= 0f)
        {
            fireCheckTimer = fireCheckInterval;
            CheckWinCondition();
        }
    }

    // Gọi từ DestinationZone.cs khi có 1 xe chạm vào vùng đích
    public void RegisterVehicleArrived(Transform vehicle)
    {
        if (levelCompleted || vehicle == null) return;

        if (arrivedVehicles.Add(vehicle))
        {
            Debug.Log($"GameManager: Xe '{vehicle.name}' đã tới đích ({arrivedVehicles.Count}/{totalVehicles}).");
            CheckWinCondition();
        }
    }

    bool FireCleared()
    {
        remainingFires = GameObject.FindGameObjectsWithTag(fireTag).Length;
        return remainingFires == 0;
    }

    bool AllVehiclesDelivered()
    {
        return totalVehicles > 0 && arrivedVehicles.Count >= totalVehicles;
    }

    public void FailMission()
    {
        if (levelCompleted) return;
        levelCompleted = true;
        LevelFail?.Invoke();
    }

    void CheckWinCondition()
    {
        if (levelCompleted) return;
        if (!FireCleared()) return;
        if (!AllVehiclesDelivered()) return;

        CompleteLevel();
    }

    void CompleteLevel()
    {
        levelCompleted = true;
        Debug.Log("GameManager: Đã dập hết lửa và cứu hết xe / Mở khóa màn 2");

        // Lưu trạng thái mở khóa màn 2, dùng cho menu chọn màn (level select)
        PlayerPrefs.SetInt("Level2Unlocked", 1);
        PlayerPrefs.Save();

        LevelComplete?.Invoke();
    }
}