using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Hamster> hamsters;

    [Header("UI objects")]
    //[SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject outOfTimeText;
    [SerializeField] private GameObject bombText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    //[SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject howToPlayPanel;

    [SerializeField] private TMPro.TextMeshProUGUI playerLivesText;

    private float startingTime = 30f;

    private float timeRemaining;
    private HashSet<Hamster> currentHamsters = new HashSet<Hamster>();
    private int score, highScore;
    private int playerLives = 3;
    private bool playing = false;

    private void Start()
    {
        
        StartGame();
    }
    public void StartGame()
    {
        //PlayerPrefs.DeleteKey("HasSeenHowToPlay");

        if (PlayerPrefs.GetInt("HasSeenHowToPlay", 0) == 1)
        {
            howToPlayPanel.SetActive(false);
            playing = true;
            Debug.Log("false");
        }
        else
        {
            howToPlayPanel.SetActive(true);
            playing = false;
            Debug.Log("true");
        }
        gameOverPanel.SetActive(false);
        outOfTimeText.SetActive(false);
        bombText.SetActive(false);
        gameUI.SetActive(true);
        mainMenuPanel.SetActive(false);

        playerLives = 3;
        UpdateLivesUI();
        Time.timeScale = 1f;
        for (int i = 0; i < hamsters.Count; i++)
        {
            hamsters[i].Hide();
            hamsters[i].SetIndex(i);
        }
        currentHamsters.Clear();
        timeRemaining = startingTime;
        score = 0;
        scoreText.text = "0";
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = highScore.ToString();
        playing = true;
    }

    public void GameOver(int type)
    {
        if (type == 0)
        {
            gameOverPanel.SetActive(true);
            outOfTimeText.SetActive(true);
        }
        else
        {
            gameOverPanel.SetActive(true);
            bombText.SetActive(true);
        }
        foreach (Hamster mole in hamsters)
        {
            mole.StopGame();
        }
        playing = false;
    }

    void Update()
    {
        if (playing)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                GameOver(0);
            }
            timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
            if (currentHamsters.Count <= (score / 1000))
            {
                int index = Random.Range(0, hamsters.Count);
                if (!currentHamsters.Contains(hamsters[index]))
                {
                    currentHamsters.Add(hamsters[index]);
                    hamsters[index].Activate(score / 1000);
                }
            }
        }
    }

    public void AddScore(int moleIndex)
    {
        score += 100;
        scoreText.text = $"{score}";
        timeRemaining += 1;
        currentHamsters.Remove(hamsters[moleIndex]);

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();

            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void Missed(int moleIndex, bool isHamster)
    {
        if (isHamster)
        {
            timeRemaining -= 2;
        }
        currentHamsters.Remove(hamsters[moleIndex]);
    }

    public void showMainMenu()
    {
        mainMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void resume()
    {
        mainMenuPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("gameScene");
    }

    public void showLeaderboards()
    {
        mainMenuPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
    }

    public void backToMenu()
    {
        mainMenuPanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        SceneManager.LoadScene("titleScene");
    }

    public void howToPlayConfirm()
    {
        playing = true;
        howToPlayPanel.SetActive(false);

        PlayerPrefs.SetInt("HasSeenHowToPlay", 1);
        PlayerPrefs.Save();
    }

    public void exitLeaderboards()
    {
        leaderboardPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void UpdateLivesUI()
    {
        playerLivesText.text = "" + playerLives;
    }

    public void LoseLife()
    {
        playerLives--;
        UpdateLivesUI();

        if (playerLives <= 0)
        {
            GameOver(1);
        }
    }
}