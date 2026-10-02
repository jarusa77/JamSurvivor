using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameModeSelectionScene = "GameModeSelect";
    [SerializeField] private string mainMenuScene = "StartSimple";

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopMusic();
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameModeSelectionScene);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}