using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using System.Collections.Generic;


/**
 * Saves the scores of all users
 */
public class Scores : MonoBehaviour {
    [SerializeField] TextMeshProUGUI usernameField;
    [SerializeField] TextMeshProUGUI textField;

    private Dictionary<string, int> scores;

    void SetScore(int newscore) {
        var username = usernameField.text;
        scores[username] = newscore;
        textField.text = "Score: " + newscore;
    }

    void Start() {
        enabled = false;
        AuthenticationService.Instance.SignedIn += async () => {
            enabled = true;
            var playerData = await DatabaseManager.LoadData("scores");
            if (playerData.TryGetValue("scores", out var scoresVar)) {
                scores = scoresVar.Value.GetAs< Dictionary<string,int> >();
                Debug.Log($"loaded scores value: {scores}");
            } else {
                scores = new();
                Debug.Log($"no score value - initializing");
            }
            var username = usernameField.text;
            if (!scores.ContainsKey(username)) {
                scores[username] = 0;
            }
            SetScore(scores[username]);
        };
    }

    public async void IncreaseScore() {
        if (enabled) {
            var username = usernameField.text;
            SetScore(scores[username] + 1);
            await DatabaseManager.SaveData(("scores", scores));
        }
    }
}
