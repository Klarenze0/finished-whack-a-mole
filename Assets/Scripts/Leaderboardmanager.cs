using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private Transform textContainer; // the textContainer with the 10 name/score rows

    private const string LeaderboardURL = "https://app-unleash-mobile-be-dev.azurewebsites.net/api/v1/play/whack-a-flea/";

    public void LoadLeaderboard()
    {
        StartCoroutine(FetchLeaderboard());
    }

    private IEnumerator FetchLeaderboard()
    {
        string token = SessionManager.AuthToken;

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogWarning("LeaderboardManager: No auth token in session.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(LeaderboardURL))
        {
            request.SetRequestHeader("Authorization", "Bearer " + token);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Leaderboard: {json}");

                LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);

                if (response != null && response.d != null && response.d.list != null)
                {
                    // Sort descending by score, take top 10
                    List<LeaderboardEntry> entries = response.d.list;
                    entries.Sort((a, b) => b.score.CompareTo(a.score));

                    int count = Mathf.Min(10, entries.Count);

                    for (int i = 0; i < textContainer.childCount; i++)
                    {
                        Transform row = textContainer.GetChild(i);
                        TextMeshProUGUI nameText  = row.GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI scoreText = row.Find("score")?.GetComponent<TextMeshProUGUI>();

                        if (i < count)
                        {
                            // Show and fill the row
                            row.gameObject.SetActive(true);

                            if (nameText  != null) nameText.text  = $"{i + 1}. {entries[i].username}";
                            if (scoreText != null) scoreText.text = entries[i].score.ToString();
                        }
                        else
                        {
                            // Hide unused rows if there are fewer than 10 entries
                            row.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch leaderboard: {request.responseCode} - {request.error}");
            }
        }
    }

    // --- JSON models ---

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
}