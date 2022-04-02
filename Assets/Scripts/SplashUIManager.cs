using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashUIManager : MonoBehaviour
{
    public GameObject loginScreen;
    public GameObject registerScreen;
    public GameObject resetPasswordScreen;

    public VideoPlayer videoPlayer;

    private void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "JustCubeBackground.mp4");
            videoPlayer.Play();
        }
    }
    public void ShowLogin()
    {
        loginScreen.SetActive(true);
        registerScreen.SetActive(false);
        resetPasswordScreen.SetActive(false);
    }

    public void ShowRegister()
    {
        loginScreen.SetActive(false);
        registerScreen.SetActive(true);
        resetPasswordScreen.SetActive(false);
    }

    public void ShowSplash()
    {
        loginScreen.SetActive(false);
        registerScreen.SetActive(false);
        resetPasswordScreen.SetActive(false);
    }

    public void ShowResetPassword()
    {
        loginScreen.SetActive(false);
        registerScreen.SetActive(false);
        resetPasswordScreen.SetActive(true);
    }
}
