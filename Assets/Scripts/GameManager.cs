using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Hamster> hamsters;

    [Header("UI objects")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject howToPlayPanel;

    [SerializeField] private TMPro.TextMeshProUGUI playerLivesText;

    private const string SaveScoreURL = "https://app-unleash-mobile-be-dev.azurewebsites.net/api/v1/users/play";
    private const string LeaderboardURL = "https://app-unleash-mobile-be-dev.azurewebsites.net/api/v1/play/whack-a-flea/";

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
        if (PlayerPrefs.GetInt("HasSeenHowToPlay", 0) == 1)
        {
            howToPlayPanel.SetActive(false);
            playing = true;
        }
        else
        {
            howToPlayPanel.SetActive(true);
            playing = false;
        }

        gameOverPanel.SetActive(false);
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
        highScore = 0;
        scoreText.text = "0";
        highScoreText.text = "0";
        playing = true;
    }

    public void GameOver(int type)
    {
        playing = false;

        foreach (Hamster mole in hamsters)
        {
            mole.StopGame();
        }

        if (type == 0)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            gameOverPanel.SetActive(true);
        }
        if (highScoreText != null)
            highScoreText.text = "";

        StartCoroutine(CheckAndSaveScore(score));
    }

    private IEnumerator CheckAndSaveScore(int finalScore)
    {
        string token = SessionManager.AuthToken;

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogWarning("CheckAndSaveScore: No auth token in session. Skipping.");
            yield break;
        }

        // GET leaderboard
        int serverHighScore = 0;
        bool playerFound = false;

        using (UnityWebRequest getRequest = UnityWebRequest.Get(LeaderboardURL))
        {
            getRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return getRequest.SendWebRequest();

            if (getRequest.result == UnityWebRequest.Result.Success)
            {
                string json = getRequest.downloadHandler.text;
                Debug.Log($"Leaderboard response: {json}");

                LeaderboardResponse leaderboard = JsonUtility.FromJson<LeaderboardResponse>(json);

                if (leaderboard != null && leaderboard.d != null && leaderboard.d.list != null)
                {
                    foreach (LeaderboardEntry entry in leaderboard.d.list)
                    {
                        if (entry.email == SessionManager.PlayerEmail)
                        {
                            serverHighScore = entry.score;
                            playerFound = true;
                            Debug.Log($"Found player on leaderboard. Server high score: {serverHighScore}");
                            break;
                        }
                    }

                    if (!playerFound)
                        Debug.Log("Player not on leaderboard yet. Will save score.");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to fetch leaderboard: {getRequest.responseCode} - {getRequest.error}. Skipping save.");
                yield break;
            }
        }

        // post if current score beats the highscore
        if (finalScore > serverHighScore)
        {
            Debug.Log($"New score {finalScore} beats server score {serverHighScore}. Saving...");
            yield return StartCoroutine(SaveScoreToServer(finalScore, token));

            if (highScoreText != null)
                highScoreText.text = finalScore.ToString();
        }
        else
        {
            Debug.Log($"Score {finalScore} does not beat server high score {serverHighScore}. Not saving.");

            if (highScoreText != null)
                highScoreText.text = serverHighScore.ToString();
        }
    }

    private IEnumerator SaveScoreToServer(int finalScore, string token)
    {
        string jsonBody = JsonUtility.ToJson(new SaveScoreRequest
        {
            name = "whack-a-flea",
            score = finalScore
        });

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(SaveScoreURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Score saved! Score: {finalScore} | Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Failed to save score: {request.responseCode} - {request.error}");
            }
        }
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
        leaderboardPanel.GetComponent<LeaderboardManager>().LoadLeaderboard();
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


    [System.Serializable]
    private class LeaderboardResponse
    {
        public int c;
        public LeaderboardData d;
    }

    [System.Serializable]
    private class LeaderboardData
    {
        public List<LeaderboardEntry> list;
    }

    [System.Serializable]
    private class LeaderboardEntry
    {
        public string id;
        public string username;
        public string email;
        public int score;
    }

    [System.Serializable]
    private class SaveScoreRequest
    {
        public string name;
        public int score;
    }
}