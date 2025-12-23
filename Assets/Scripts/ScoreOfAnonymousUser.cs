using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

/**
 * Saves the score of an anonymous user
 */
public class ScoreOfAnonymousUser : MonoBehaviour { 
    [SerializeField] TextMeshProUGUI textField;

    private int score = -1;   // not initialized

    private void Awake()
    {
        enabled = false;
    }


    /* Initialization */
    void Start()
    {
        enabled = false;
        AuthenticationService.Instance.SignedIn += Initialize;
    }

    async void Initialize() {
        int loadedScore;
        var scoreData = await DatabaseManager.LoadData("score");
        if (scoreData.TryGetValue("score", out var scoreVar))
        {
            loadedScore = scoreVar.Value.GetAs<int>();
            Debug.Log($"loaded score value: {score}");
        } else
        {
            loadedScore = 0;
            Debug.Log($"no score value - initializing to {score}");
        }
        SetScore(loadedScore);
        gameObject.SetActive(true);
        enabled = true;
    }


    /* Changing the score */

    void SetScore(int newscore)
    {
        score = newscore;
        textField.text = "Score: " + score;
    }

    public async void IncreaseScore() {
        if (enabled) {
            SetScore(score + 1);
            await DatabaseManager.SaveData(("score", score));
        }
    }

}
