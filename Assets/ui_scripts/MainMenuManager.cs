
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("IntroVideo");
    }

    public void ExitGame()
    {
        Debug.Log("Cerrando juego...");
        Application.Quit();
    }
}
