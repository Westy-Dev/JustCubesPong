using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.ServerModels;
using PlayFab.Internal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Linq;
using System.Text;

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

        var request = new PlayFab.ClientModels.RegisterPlayFabUserRequest
        {
            Email = registerEmailInput.text,
            Username = registerUsernameInput.text,
            DisplayName = registerUsernameInput.text,
            Password = registerPasswordInput.text,
            RequireBothUsernameAndEmail = false
        };
        Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("RegisterPlayFabUser");
        extraHeaders.Add("C0", requestVal);
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterError, null, extraHeaders);
    }

    void OnRegisterSuccess(PlayFab.ClientModels.RegisterPlayFabUserResult result)
    {
        messageText.text = "Succesfully registered!";
    }

    void OnRegisterError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log("Error registering : " + error.GenerateErrorReport());
    }


   //Encryption function starts.
    public static string EncryptString(string PlainText)
    {
      var key = "b14ca5898a4e4133bbce2ea2315a1916";
      byte[] iv = new byte[16];
      byte[] array;

      using (Aes aes = Aes.Create())
      {
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
            {
              streamWriter.Write(PlainText);
            }

            array = memoryStream.ToArray();
          }
        }
      }

      return Convert.ToBase64String(array);
    }
    //Encryption function ends.

    // Update is called once per frame
    public void LoginButton()
    {
        var request = new PlayFab.ClientModels.LoginWithPlayFabRequest
        {
            Username = loginUsernameInput.text,
            Password = loginPasswordInput.text,
            InfoRequestParameters = new PlayFab.ClientModels.GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("LoginWithPlayFab");
        extraHeaders.Add("C0", requestVal);
        PlayFabClientAPI.LoginWithPlayFab(request, onLoginSuccess, onLoginError, null, extraHeaders);
    }

    void onLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("Successful login/account create!");
        SceneManager.LoadScene("Pong Game");
        PlayerPrefs.SetString("playFabId", result.PlayFabId);
    }

    void onLoginError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account");
    if(error.ErrorMessage == "The account making this request is currently banned")
      messageText.text = "Daily play limit of 30 tries exceeded. Please try tomorrow";
    else
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    public void PasswordResetButton()
    {
        var request = new PlayFab.ClientModels.SendAccountRecoveryEmailRequest
        {
            Email = resetPasswordEmailInput.text,
            TitleId = TitleID
        };
        Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("SendAccountRecoveryEmail");
        extraHeaders.Add("C0", requestVal);
        PlayFabClientAPI.SendAccountRecoveryEmail(request, onPasswordResetSuccess, onPasswordResetError, null, extraHeaders);
    }

    void onPasswordResetSuccess(PlayFab.ClientModels.SendAccountRecoveryEmailResult result)
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
    var val1 = 1;
        var request = new PlayFab.ClientModels.UpdatePlayerStatisticsRequest
        {
            Statistics = new List<PlayFab.ClientModels.StatisticUpdate>
            {
                new PlayFab.ClientModels.StatisticUpdate
                {
                    StatisticName = "Score",
                    Value = score
                    
                },
                new PlayFab.ClientModels.StatisticUpdate
                {
                    StatisticName = "sessionCounter",
                    Value = 1 //This will work as an increment counter and will increase the value in PlayFab by 1.
                }
            }
        };
        Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("UpdatePlayerStatistics");
        extraHeaders.Add("C0", requestVal);

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnLeaderboardError, null, extraHeaders);
    }

    void OnLeaderboardUpdate(PlayFab.ClientModels.UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful Leaderboard Sent");
    }

    void OnLeaderboardError(PlayFabError error)
    {
        Debug.Log("Error when updating leaderboard : " + error.GenerateErrorReport());
    }

    public void GetLeaderBoard()
    {
        var request = new PlayFab.ClientModels.GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 100
        };
        var requestVal = EncryptString("GetLeaderBoard");
        Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        extraHeaders.Add("C0", requestVal);
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnGetLeaderboardError, null, extraHeaders);
    }

    void OnLeaderboardGet(PlayFab.ClientModels.GetLeaderboardResult result)
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
        var request = new PlayFab.ClientModels.GetPlayerStatisticsRequest
        {
            StatisticNames = new List<string> { "Score", "sessionCounter" }
        };
        Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("GetPlayerStatistics");
        extraHeaders.Add("C0", requestVal);
        PlayFabClientAPI.GetPlayerStatistics(request, OnHighScoreGet, onHighScoreError, null, extraHeaders);
    }

    void OnHighScoreGet(PlayFab.ClientModels.GetPlayerStatisticsResult result)
    {
      for(int i = 0; i < result.Statistics.Count; i++)
      {
        if(result.Statistics[i].StatisticName == "Score")
        {
          Debug.Log("Obtained Highscore = " + result.Statistics[i].Value);
          if (PlayerBestScore != null)
          {
              PlayerBestScore.GetComponent<Text>().text = "Your Best: " + result.Statistics[i].Value;
          }

          if(PlayerBestScoreGameOver != null)
          {
              PlayerBestScoreGameOver.GetComponent<Text>().text = "Your Best: " + result.Statistics[i].Value;
          }
        }
        if(result.Statistics[i].StatisticName == "sessionCounter")
        {
          if (result.Statistics[i].Value >= PlayFabSettings.staticSettings.SessionCounter)
          {
            string playFabId = PlayerPrefs.GetString("playFabId");
            AddBan(playFabId);
          }
        }
      } 
    }

 
    void OnBanUserSuccess(PlayFab.ServerModels.BanUsersResult result)
    {
    var request = new PlayFab.ServerModels.UpdateBansRequest
        {
             Bans = new List<UpdateBanRequest>() {
                new UpdateBanRequest() {
                    BanId = result.BanData[0].BanId,
                    Expires = DateTime.UtcNow.Date.AddDays(1), // It will ban the player for rest of the day.
                    Reason = "Play time exceeded"
                }
            }
        };
    Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("UpdateBans");
        extraHeaders.Add("C0", requestVal);
    PlayFabServerAPI.UpdateBans(request, OnUpdateBanUserSuccess, OnUpdateBanUserFailure, null, extraHeaders);
        
    }

    void OnUpdateBanUserSuccess(PlayFab.ServerModels.UpdateBansResult result)
    {
        Debug.Log("You are banned from playing because your maximum number of tries for a day has exceeded."); 
        SceneManager.LoadScene("Login Screen");
    }

    void OnUpdateBanUserFailure(PlayFabError error)
    {
        Debug.Log("Error when banning the player : " + error.GenerateErrorReport());
    } 

    void OnBanUserFailure(PlayFabError error)
    {
        Debug.Log("Error when banning the player : " + error.GenerateErrorReport());
    } 

    void onHighScoreError(PlayFabError error)
    {
        Debug.Log("Error when obtaining high score : " + error.GenerateErrorReport());
    }

public void AddBan(string playFabId) {
    string currentTime = DateTime.UtcNow.ToString("hh:mm tt");
    int totalMinutes = 0;
    if (currentTime.Contains("PM"))
            {
                int hours = Convert.ToInt32(currentTime.Substring(0, 2));
                int HoursInminutes = hours * 60;
                string minutesInString = currentTime.Split(':')[1];
                int minutes = Convert.ToInt32(minutesInString.Remove(2));
                totalMinutes = HoursInminutes + minutes;
                Console.WriteLine(totalMinutes);
            }
            else
            {
                int hours = Convert.ToInt32(currentTime.Substring(0, 2));
                int HoursInminutes = (hours % 60) * 60;
                string minutesInString = currentTime.Split(':')[1];
                int minutes = Convert.ToInt32(minutesInString.Remove(2));
                totalMinutes = HoursInminutes + minutes;
                Console.WriteLine(totalMinutes);
            }
    uint remaining = Convert.ToUInt32(1440 - totalMinutes)/60;
    var request = new PlayFab.ServerModels.BanUsersRequest
        {
             Bans = new List<BanRequest>() {
                new BanRequest() {
                    DurationInHours = remaining,
                    PlayFabId = playFabId,
                    Reason = "Play time exceeded",
                }
            }
        };
    Dictionary<string, string> extraHeaders = new Dictionary<string, string>();
        var requestVal = EncryptString("BanUsers");
        extraHeaders.Add("C0", requestVal);
    PlayFabServerAPI.BanUsers(request, OnBanUserSuccess, OnBanUserFailure, null, extraHeaders);
}

}
