using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionResultUI : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private GameObject fireCheckIcon;
    [SerializeField] private GameObject fireFailIcon;

    [SerializeField] private GameObject victimCheckIcon;
    [SerializeField] private GameObject victimFailIcon;

    [SerializeField] private GameObject vehicleCheckIcon;
    [SerializeField] private GameObject vehicleFailIcon;

    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private string nextLevelSceneName = "Level2";

    void Start()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    void setIcon(GameObject CheckIcon, GameObject FailIcon, bool Done)
    {
        if (Done == true)
        {
            CheckIcon.SetActive(true);
            FailIcon.SetActive(false);
        }
        else
        {
            CheckIcon.SetActive(false);
            FailIcon.SetActive(true);
        }
    }

    public void CompleteLevel()
    {
        if (GameManager.Instance == null) return;
        resultPanel.SetActive(true);
        CheckStatus();
        nextLevelButton.SetActive(true);
    }

    public void FailLevel()
    {
        if (GameManager.Instance == null) return;
        resultPanel.SetActive(true);
        CheckStatus();
        nextLevelButton.SetActive(false);
    }

    private void CheckStatus()
    {
        bool firedone;
        bool victimdone;
        bool vehicledone;
        if (GameManager.Instance.RemainingFires <= 0)
        {
            firedone = true;
        }
        else
        {
            firedone = false;
        }
        setIcon(fireCheckIcon, fireFailIcon, firedone);

        if (GameManager.Instance.RemainingVictims <= 0)
        {
            victimdone = true;
        }
        else
        {
            victimdone = false;
        }
        setIcon(victimCheckIcon, victimFailIcon, victimdone);

        if (GameManager.Instance.RemainingVehicles <= 0)
        {
            vehicledone = true;
        }
        else
        {
            vehicledone = false;
        }
        setIcon(vehicleCheckIcon, vehicleFailIcon, vehicledone);
    }
    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelSceneName);
    }
}
