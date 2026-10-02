using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelectionUI : MonoBehaviour
{
    private const string GameModeKey = "SelectedGameMode";

    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string mainMenuSceneName = "StartSimple";

    [Header("Optional UI")]
    [SerializeField] private GameObject soloSelectedIndicator;
    [SerializeField] private GameObject multiplayerSelectedIndicator;
    [SerializeField] private GameObject confirmPanel;

    private GameMode selectedMode = GameMode.Solo;

    private void Start()
    {
        selectedMode = (GameMode)PlayerPrefs.GetInt(
            GameModeKey,
            (int)GameMode.Solo);

        UpdateSelectionUI();
    }

    public void SelectSoloMode()
    {
        selectedMode = GameMode.Solo;
        UpdateSelectionUI();
    }

    public void SelectMultiplayerMode()
    {
        selectedMode = GameMode.LocalMultiplayer;
        UpdateSelectionUI();
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt(GameModeKey, (int)selectedMode);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void UpdateSelectionUI()
    {
        if (soloSelectedIndicator != null)
        {
            soloSelectedIndicator.SetActive(selectedMode == GameMode.Solo);
        }

        if (multiplayerSelectedIndicator != null)
        {
            multiplayerSelectedIndicator.SetActive(
                selectedMode == GameMode.LocalMultiplayer);
        }

        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
        }
    }
    public enum GameMode
    {
        Solo = 0,
        LocalMultiplayer = 1
    }
}
