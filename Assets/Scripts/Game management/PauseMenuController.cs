using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Panel Paused")]
    public GameObject pausePanel;

    [Header("MainMenu")]
    public string MainMenu = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(MainMenu);
    }

    public void OpenSettings(GameObject settingsPanel)
    {
        settingsPanel.SetActive(true);
    }
}