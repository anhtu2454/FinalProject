using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject levelsPanel;

    [Header("Tên scene của level 1 ")]
    public string firstLevelSceneName = "Level1";

    public void NewGame()
    {
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OpenLevelSelect()
    {
        levelsPanel.SetActive(true);
    }

    public void CloseLevelSelect()
    {
        levelsPanel.SetActive(false);
    }

    public void LoadLevel(int levelNumber)
    {
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