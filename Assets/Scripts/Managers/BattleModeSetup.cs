using UnityEngine;
using static GameModeSelectionUI;

[DefaultExecutionOrder(-100)]
public class BattleModeSetup : MonoBehaviour
{
    private const string GameModeKey = "SelectedGameMode";

    [Header("Fighters")]
    [SerializeField] private Fighter playerOne;
    [SerializeField] private Fighter playerTwo;

    [Header("AI")]
    [SerializeField] private SimpleFighterAI playerTwoAI;

    private void Awake()
    {
        GameMode selectedMode = (GameMode)PlayerPrefs.GetInt(
            GameModeKey,
            (int)GameMode.Solo);

        bool isSoloMode = selectedMode == GameMode.Solo;

        playerOne.SetAIControlled(false);
        playerTwo.SetAIControlled(isSoloMode);

        if (playerTwoAI != null)
        {
            playerTwoAI.enabled = isSoloMode;
        }

        Debug.Log(isSoloMode
            ? "Started Solo Mode: Player 2 is AI."
            : "Started Local Multiplayer: Player 2 is human-controlled.");
    }
}
