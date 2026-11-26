
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Referencias")]
    public VideoPlayer videoPlayer;

    [Header("Escena del juego")]
    public string gameSceneName = "mapa1";

    private bool hasStartedGame = false;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadGame();
    }

    public void SkipVideo()
    {
        LoadGame();
    }

    void LoadGame()
    {
        if (hasStartedGame) return;

        hasStartedGame = true;

        SceneManager.LoadScene(gameSceneName);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}
