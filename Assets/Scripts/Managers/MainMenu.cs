using UnityEngine;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        if(SoundManager.Instance!=null)
            SoundManager.Instance.StopMusic();
    }

    public string GameScene = "Game";
    public void LoadGame()
    {
        SceneManager.LoadScene(GameScene); // replace with your scene name
    }

    public void LoadMainMenu()
    {
        Debug.Log("LoanMain");
        SceneManager.LoadScene("StartSimple");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // shows in editor
    }
}