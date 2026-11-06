using UnityEngine;
using UnityEngine.UI;

public class VictimHealth : MonoBehaviour
{
    [Tooltip("Máu tối đa")]
    public float maxHealth = 100f;

    [Header("Thanh máu")]
    [Tooltip("Chiều rộng thanh máu")]
    public float barWidth = 1.5f;

    [Tooltip("Chiều cao thanh máu")]
    public float barHeight = 0.2f;

    [Tooltip("Thanh máu nằm cao hơn victim bao nhiêu")]
    public float heightAboveVictim = 2.2f;

    private float currentHealth;
    private bool isDead = false;

    private GameObject barRoot;
    private RectTransform fillRect;
    private Image fillImage;
    private Camera mainCamera;

    public float CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        CreateHealthBar();
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void CreateHealthBar()
    {
        // object gốc của thanh máu
        barRoot = new GameObject("HealthBar");
        Canvas canvas = barRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        barRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight);

        // nền
        GameObject background = new GameObject("Background");
        background.transform.SetParent(barRoot.transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.6f);
        Stretch(background.GetComponent<RectTransform>());

        // thanh máu
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(barRoot.transform, false);
        fillImage = fill.AddComponent<Image>();
        fillRect = fill.GetComponent<RectTransform>();
        Stretch(fillRect);

        UpdateBar();
        barRoot.SetActive(false); // ẩn cho tới khi bị trừ máu
    }

    void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void UpdateBar()
    {
        float ratio = Mathf.Clamp01(currentHealth / maxHealth);
        fillRect.anchorMax = new Vector2(ratio, 1f);
        fillImage.color = Color.Lerp(Color.red, Color.green, ratio);
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f) return;

        currentHealth -= amount;

        if (!barRoot.activeSelf)
        {
            barRoot.SetActive(true);
        }

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            UpdateBar();

            isDead = true;
            Debug.Log($"VictimHealth: '{name}' đã hết máu!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.FailMission();
            }
            return;
        }

        UpdateBar();
    }

    // đặt vị trí và hướng thanh máu sau khi victim di chuyển
    void LateUpdate()
    {
        if (barRoot == null || !barRoot.activeSelf) return;

        barRoot.transform.position = transform.position + Vector3.up * heightAboveVictim;

        if (mainCamera != null)
        {
            barRoot.transform.rotation = mainCamera.transform.rotation;
        }
    }

    // victim được cứu xóa thanh máu
    void OnDestroy()
    {
        if (barRoot != null)
        {
            Destroy(barRoot);
        }
    }
}