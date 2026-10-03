using UnityEngine;
using UnityEngine.SceneManagement;

public class GameModeSelectionUI : MonoBehaviour
{
    private const string GameModeKey = "SelectedGameMode";

    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string mainMenuSceneName = "StartSimple";

    [Header("Selection Display")]
    [SerializeField] private GameObject soloSelectedText;
    [SerializeField] private GameObject multiplayerSelectedText;

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

        Debug.Log("Solo mode selected.");
    }

    public void SelectMultiplayerMode()
    {
        selectedMode = GameMode.LocalMultiplayer;
        UpdateSelectionUI();

        Debug.Log("Local multiplayer selected.");
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
        if (soloSelectedText != null)
        {
            soloSelectedText.SetActive(selectedMode == GameMode.Solo);
        }

        if (multiplayerSelectedText != null)
        {
            multiplayerSelectedText.SetActive(
                selectedMode == GameMode.LocalMultiplayer);
        }
    }

    public enum GameMode
    {
        Solo = 0,
        LocalMultiplayer = 1
    }
}
