using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGameCreditsLoader : MonoBehaviour
{
    [Header("Credits Settings")]
    public string creditsSceneName = "Credits";
    public float delayBeforeLoad = 0f;

    // Call this when the game ends
    public void EndGame()
    {
        if (delayBeforeLoad > 0f)
            StartCoroutine(LoadCreditsAfterDelay());
        else
            LoadCredits();
    }

    void LoadCredits()
    {
        SceneManager.LoadScene(creditsSceneName);
    }

    IEnumerator LoadCreditsAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        LoadCredits();
    }
}
