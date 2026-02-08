using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [SerializeField] private List<Hamster> hamsters;

  [Header("UI objects")]
  [SerializeField] private GameObject playButton;
  [SerializeField] private GameObject gameUI;
  [SerializeField] private GameObject gameOverPanel;
  [SerializeField] private GameObject outOfTimeText;
  [SerializeField] private GameObject bombText;
  [SerializeField] private TMPro.TextMeshProUGUI timeText;
  [SerializeField] private TMPro.TextMeshProUGUI scoreText;
  [SerializeField] private TMPro.TextMeshProUGUI highScoreText;

    private float startingTime = 30f;

  private float timeRemaining;
  private HashSet<Hamster> currentHamsters = new HashSet<Hamster>();
  private int score, highScore;

  private bool playing = false;


  public void StartGame() {

        gameOverPanel.SetActive(false);
    playButton.SetActive(false);
    outOfTimeText.SetActive(false);
    bombText.SetActive(false);
    gameUI.SetActive(true);
    for (int i = 0; i < hamsters.Count; i++) {
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

  public void GameOver(int type) {
    if (type == 0) {
            gameOverPanel.SetActive(true);
      outOfTimeText.SetActive(true);
    } else {
            gameOverPanel.SetActive(true);
            bombText.SetActive(true);
    }
    foreach (Hamster mole in hamsters) {
      mole.StopGame();
    }
    playing = false;
    playButton.SetActive(true);
  }

  void Update() {
    if (playing) {
      timeRemaining -= Time.deltaTime;
      if (timeRemaining <= 0) {
        timeRemaining = 0;
        GameOver(0);
      }
      timeText.text = $"{(int)timeRemaining / 60}:{(int)timeRemaining % 60:D2}";
      if (currentHamsters.Count <= (score / 10)) {
        int index = Random.Range(0, hamsters.Count);
        if (!currentHamsters.Contains(hamsters[index])) {
          currentHamsters.Add(hamsters[index]);
          hamsters[index].Activate(score / 10);
        }
      }
    }
  }

    public void AddScore(int moleIndex) {
        score += 1;
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

  public void Missed(int moleIndex, bool isHamster) {
    if (isHamster) {
      timeRemaining -= 2;
    }
    currentHamsters.Remove(hamsters[moleIndex]);
  }
}