using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{
    [Header("UI")]
    public Text messageText;

    public InputField loginUsernameInput;
    public InputField loginPasswordInput;

    public InputField registerUsernameInput;
    public InputField registerPasswordInput;
    public InputField registerPasswordInput2;
    public InputField registerEmailInput;

    public InputField resetPasswordEmailInput;
  

    private string TitleID = "2C11F";

    public GameObject LeaderboardRowPrefab;
    public Transform LeaderboardRowsParent;
    public GameObject PlayerBestScore;
    public GameObject PlayerBestScoreGameOver;

    [SerializeField]
    private AudioSource musicAudioSource;

    [SerializeField]
    private AudioSource soundFXAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (musicAudioSource != null)
        {
            float musicVolumeValue;
            if (PlayerPrefs.HasKey("MusicVolumeValue"))
            {
                musicVolumeValue = PlayerPrefs.GetFloat("MusicVolumeValue");
            } else
            {
                musicVolumeValue = 1.0f;
            }
            musicAudioSource.volume = musicVolumeValue;
        }

        if (soundFXAudioSource != null)
        {
            float soundFXVolume;
            if (PlayerPrefs.HasKey("SoundFXVolumeValue"))
            {
                soundFXVolume = PlayerPrefs.GetFloat("SoundFXVolumeValue");
            }
            else
            {
                soundFXVolume = 1.0f;
            }
            soundFXAudioSource.volume = soundFXVolume;
        }
    }

    public void RegisterButton()
    {
        if(registerPasswordInput.text.Length < 6)
        {
            messageText.text = "Password needs to be atleast 5 characters";
            return;
        }

        if (!registerPasswordInput.text.Equals(registerPasswordInput2.text))
        {
            messageText.text = "Passwords must match";
            return;
        }

        if(registerUsernameInput.text.Length > 32)
        {
            messageText.text = "Username cannot be longer than 32 characters";
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = registerEmailInput.text,
            Username = registerUsernameInput.text,
            DisplayName = registerUsernameInput.text,
            Password = registerPasswordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Succesfully registered!";
    }

    void OnRegisterError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log("Error registering : " + error.GenerateErrorReport());
    }

    // Update is called once per frame
    public void LoginButton()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = loginUsernameInput.text,
            Password = loginPasswordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithPlayFab(request, onLoginSuccess, onLoginError);
    }

    void onLoginSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
        SceneManager.LoadScene("Pong Game");
    }

    void onLoginError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account");
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    public void PasswordResetButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = resetPasswordEmailInput.text,
            TitleId = TitleID
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, onPasswordResetSuccess, onPasswordResetError);
    }

    void onPasswordResetSuccess(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Password reset email sent!");
        messageText.text = "Password reset email sent!";
    }

    void onPasswordResetError(PlayFabError error)
    {
        Debug.Log("Error while setting password");
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnLeaderboardError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful Leaderboard Sent");
    }

    void OnLeaderboardError(PlayFabError error)
    {
        Debug.Log("Error when updating leaderboard : " + error.GenerateErrorReport());
    }

    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnGetLeaderboardError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        Debug.Log("Obtained Leaderboard");
        foreach(var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(LeaderboardRowPrefab, LeaderboardRowsParent);
            Text[] texts = newRow.GetComponentsInChildren<Text>();
            texts[0].text = "#" + ((item.Position+1) .ToString());
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            Debug.Log(item.Position + " " + item.DisplayName + " " + item.StatValue);
        }
    }

    void OnGetLeaderboardError(PlayFabError error)
    {
        Debug.Log("Error when obtaining leaderboard : " + error.GenerateErrorReport());
    }

    public void GetUserHighScore()
    {
        var request = new GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { "Score" }
        };

        PlayFabClientAPI.GetPlayerStatistics(request, OnHighScoreGet, onHighScoreError);
    }

    void OnHighScoreGet(GetPlayerStatisticsResult result)
    {
        Debug.Log("Obtained Highscore = " + result.Statistics[0].Value);

        if (PlayerBestScore != null)
        {
            PlayerBestScore.GetComponent<Text>().text = "Your Best: " + result.Statistics[0].Value;
        }

        if(PlayerBestScoreGameOver != null)
        {
            PlayerBestScoreGameOver.GetComponent<Text>().text = "Your Best: " + result.Statistics[0].Value;
        }
    }

    void onHighScoreError(PlayFabError error)
    {
        Debug.Log("Error when obtaining high score : " + error.GenerateErrorReport());
    }

}
