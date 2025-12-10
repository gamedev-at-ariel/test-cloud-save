using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

/**
 * Saves the score of an anonymous user
 */
public class Score : MonoBehaviour { 
    [SerializeField] TextMeshProUGUI textField;

    private int score = -1;   // not initialized


    /* Initialization */
    void Start()
    {
        enabled = false;
        AuthenticationService.Instance.SignedIn += OnSignedIn;
    }

    async void OnSignedIn() {
        int loadedScore = await DatabaseManager.LoadScore();
        SetScore(loadedScore);
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
            await DatabaseManager.SaveScore(score);
        }
    }

}
