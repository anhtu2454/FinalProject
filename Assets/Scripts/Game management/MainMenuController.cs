using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject levelsPanel;

    [Header("5 nút chọn màn, theo đúng thứ tự Level1 -> Level5")]
    public Button[] levelButtons;

    [Header("Độ mờ của nút bị khóa")]
    [Range(0f, 1f)]
    public float lockedAlpha = 0.4f;

    [Header("Tên scene của level 1 ")]
    public string firstLevelSceneName = "Level1";

    void Start()
    {
        RefreshLevelButtons();
    }

    // Nút nào chưa mở thì mờ đi và không bấm được
    void RefreshLevelButtons()
    {
        int unlocked = LevelProgress.GetUnlockedLevel();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button btn = levelButtons[i];
            if (btn == null) continue;

            bool isUnlocked = (i + 1) <= unlocked;
            btn.interactable = isUnlocked;

            if (btn.image != null)
            {
                Color c = btn.image.color;
                if (isUnlocked)
                {
                    c.a = 1f;
                }
                else
                {
                    c.a = lockedAlpha;
                }
                btn.image.color = c;
            }
        }
    }

    public void NewGame()
    {
        LevelProgress.ResetProgress();
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OpenLevelSelect()
    {
        RefreshLevelButtons();
        levelsPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        levelsPanel.SetActive(false);
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber > LevelProgress.GetUnlockedLevel()) return;

        SceneManager.LoadScene("Level" + levelNumber);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}