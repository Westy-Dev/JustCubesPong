using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayFabManager playfabManager;
    [SerializeField] Text playerScoreText;

    int playerScore = 0;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject startGamePanel;
    [SerializeField] Text gameOverPlayerScoreText;

    int scoreToAdd = 1;
    int scoreHitCount = 3;
    int count = 0;

    bool canRestart = false;
    bool canStartGame = false;

    [SerializeField]
    private List<AudioClip> gameMusicTracks;
    [SerializeField]
    private AudioSource musicAudioSource;

    private void Awake()
    {
        Instance = this;

        startGamePanel.SetActive(true);
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        canStartGame = true;
        gameOverPanel.SetActive(false);
        UpdateScore();
        SelectMusic();
    }

    private void Update()
    {
        if (canRestart) {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Restart();
            }
        }
        if (canStartGame)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayGame();
            }
        }
    }

    public void AddPaddleCollisionScore()
    {
        count++;
        if (count == scoreHitCount)
        {
            count = 0;
            scoreToAdd++;
        }

        AddPlayerScore(scoreToAdd);
    }

    public void AddPlayerScore(int ScoreToAdd)
    {
        playerScore += ScoreToAdd;
        UpdateScore();
    }

    void UpdateScore()
    {
        playerScoreText.text = "Score: " + playerScore.ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        gameOverPlayerScoreText.text = "Score: " + playerScore.ToString();
        playfabManager.SendLeaderboard(playerScore);
        playfabManager.GetUserHighScore();
        canRestart = true;
        canStartGame = true;
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }
    }

    public void Restart()
    {
        canRestart = false;
        playfabManager.GetUserHighScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayGame()
    {
        canStartGame = false;
        playfabManager.GetUserHighScore();
        startGamePanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void SelectMusic()
    {

        if(gameMusicTracks.Count > 0)
        {
            int index = Random.Range(0, gameMusicTracks.Count);
            AudioClip audioToPlay = gameMusicTracks[index];
            if (musicAudioSource != null)
            {
                musicAudioSource.clip = audioToPlay;
                musicAudioSource.Play();
            }
        }
    }
}
