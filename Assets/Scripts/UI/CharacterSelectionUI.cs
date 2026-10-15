
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [Serializable]
    public class CharacterOption
    {
        public string characterName;
        public Sprite portrait;
    }

    private const string ModeKey = "SelectedGameMode";
    private const string PlayerOneKey = "Player1Character";
    private const string PlayerTwoKey = "Player2Character";

    [Header("Panels")]
    [SerializeField] private GameObject gameModePanel;
    [SerializeField] private GameObject characterSelectionPanel;

    [Header("Battle Scene")]
    [SerializeField] private string gameSceneName = "Game";

    [Header("Available Characters")]
    [SerializeField] private CharacterOption[] characters;

    [Header("Player 1 Display")]
    [SerializeField] private TMP_Text playerOneCharacterName;
    [SerializeField] private Image playerOnePortrait;

    [Header("Player 2 / AI Display")]
    [SerializeField] private TMP_Text opponentLabel;
    [SerializeField] private TMP_Text playerTwoCharacterName;
    [SerializeField] private Image playerTwoPortrait;

    private int playerOneIndex;
    private int playerTwoIndex;

    private bool HasCharacters =>
        characters != null && characters.Length > 0;

    private void Awake()
    {
        if (characterSelectionPanel != null)
            characterSelectionPanel.SetActive(false);

        if (gameModePanel != null)
            gameModePanel.SetActive(true);
    }

    public void OpenCharacterSelection()
    {
        if (!HasCharacters)
        {
            Debug.LogError(
                "CharacterSelectionUI: Add characters in the Inspector.",
                this);
            return;
        }

        playerOneIndex = Mathf.Clamp(
            PlayerPrefs.GetInt(PlayerOneKey, 0),
            0,
            characters.Length - 1);

        playerTwoIndex = Mathf.Clamp(
            PlayerPrefs.GetInt(PlayerTwoKey, 0),
            0,
            characters.Length - 1);

        if (opponentLabel != null)
        {
            bool isSoloMode = PlayerPrefs.GetInt(ModeKey, 0) == 0;

            opponentLabel.text = isSoloMode
                ? "AI Character"
                : "Player 2 Character";
        }

        UpdateDisplays();

        if (gameModePanel != null)
            gameModePanel.SetActive(false);

        if (characterSelectionPanel != null)
            characterSelectionPanel.SetActive(true);
    }

    public void NextPlayerOne()
    {
        if (!HasCharacters)
            return;

        playerOneIndex = (playerOneIndex + 1) % characters.Length;
        UpdateDisplays();
    }

    public void PreviousPlayerOne()
    {
        if (!HasCharacters)
            return;

        playerOneIndex =
            (playerOneIndex - 1 + characters.Length) % characters.Length;

        UpdateDisplays();
    }

    public void NextPlayerTwo()
    {
        if (!HasCharacters)
            return;

        playerTwoIndex = (playerTwoIndex + 1) % characters.Length;
        UpdateDisplays();
    }

    public void PreviousPlayerTwo()
    {
        if (!HasCharacters)
            return;

        playerTwoIndex =
            (playerTwoIndex - 1 + characters.Length) % characters.Length;

        UpdateDisplays();
    }

    public void StartBattle()
    {
        if (!HasCharacters)
            return;

        PlayerPrefs.SetInt(PlayerOneKey, playerOneIndex);
        PlayerPrefs.SetInt(PlayerTwoKey, playerTwoIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    public void BackToGameMode()
    {
        if (characterSelectionPanel != null)
            characterSelectionPanel.SetActive(false);

        if (gameModePanel != null)
            gameModePanel.SetActive(true);
    }

    private void UpdateDisplays()
    {
        UpdateCharacterDisplay(
            playerOneIndex,
            playerOneCharacterName,
            playerOnePortrait);

        UpdateCharacterDisplay(
            playerTwoIndex,
            playerTwoCharacterName,
            playerTwoPortrait);
    }

    private void UpdateCharacterDisplay(
        int index,
        TMP_Text nameText,
        Image portraitImage)
    {
        CharacterOption character = characters[index];

        if (nameText != null)
            nameText.text = character.characterName;

        if (portraitImage != null)
        {
            portraitImage.sprite = character.portrait;
            portraitImage.enabled = character.portrait != null;
        }
    }
}