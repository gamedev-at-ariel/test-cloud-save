using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

/**
 * Saves the score of a single user
 */
public class Score : MonoBehaviour { 
    [SerializeField] TextMeshProUGUI textField;

    private int score = -1;   // not initialized

    void SetScore(int newscore) {
        score = newscore;
        textField.text = "Score: " + score;
    }

    void Start() {
        enabled = false;
        AuthenticationService.Instance.SignedIn += async () => {
            SetScore(await DatabaseManager.LoadScore());
            enabled = true; 
        };
    }

    public async void IncreaseScore() {
        if (enabled) {
            SetScore(score + 1);
            await DatabaseManager.SaveScore(score);
        }
    }

}
