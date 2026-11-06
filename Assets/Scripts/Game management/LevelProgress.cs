using UnityEngine;
using UnityEngine.SceneManagement;


public static class LevelProgress
{
    public const int MaxLevel = 5;

    private const string UnlockedKey = "UnlockedLevel";

    public static int GetUnlockedLevel()
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(UnlockedKey, 1), 1, MaxLevel);
    }

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(UnlockedKey, 1);
        PlayerPrefs.Save();
    }


    public static int GetCurrentLevelNumber()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (!sceneName.StartsWith("Level")) return 0;

        int number;
        if (int.TryParse(sceneName.Substring(5), out number))
        {
            return number;
        }
        return 0;
    }

    public static bool IsLastLevel()
    {
        return GetCurrentLevelNumber() >= MaxLevel;
    }

    public static void UnlockNextLevel()
    {
        int current = GetCurrentLevelNumber();
        if (current <= 0) return;

        int next = Mathf.Min(current + 1, MaxLevel);
        if (next > GetUnlockedLevel())
        {
            PlayerPrefs.SetInt(UnlockedKey, next);
            PlayerPrefs.Save();
        }
    }
}