using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using System.Collections.Generic;


/**
 * Saves the scores of all users using username+password authentication
 */
public class Scores : MonoBehaviour {
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] TextMeshProUGUI statusField;
    [SerializeField] GameObject signInPanel;
    [SerializeField] GameObject gamePanel;

    private Dictionary<string, int> scores;
    private AuthenticationManagerWithPassword authManager;

    void SetScore(int newscore) {
        var username = usernameInputField.text;
        scores[username] = newscore;
        textField.text = "Score: " + newscore;
    }

    void Start() {
        authManager = FindAnyObjectByType<AuthenticationManagerWithPassword>();
        enabled = false;

        // Show sign-in panel, hide game panel initially
        if (signInPanel != null) signInPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);

        AuthenticationService.Instance.SignedIn += async () => {
            enabled = true;

            // Hide sign-in panel, show game panel
            if (signInPanel != null) signInPanel.SetActive(false);
            if (gamePanel != null) gamePanel.SetActive(true);

            var playerData = await DatabaseManager.LoadData("scores");
            if (playerData.TryGetValue("scores", out var scoresVar)) {
                scores = scoresVar.Value.GetAs< Dictionary<string,int> >();
                Debug.Log($"loaded scores value: {scores}");
            } else {
                scores = new();
                Debug.Log($"no score value - initializing");
            }
            var username = usernameInputField.text;
            if (!scores.ContainsKey(username)) {
                scores[username] = 0;
            }
            SetScore(scores[username]);
        };
    }

    public async void OnSignInButtonClicked() {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            if (statusField != null) statusField.text = "Please enter username and password";
            return;
        }

        if (statusField != null) statusField.text = "Signing in...";
        bool success = await authManager.SignInWithUsernamePassword(username, password);

        if (!success && statusField != null) {
            statusField.text = "Sign in failed. Check console for details.";
        }
    }

    public async void OnSignUpButtonClicked() {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            if (statusField != null) statusField.text = "Please enter username and password";
            return;
        }

        if (statusField != null) statusField.text = "Signing up...";
        bool success = await authManager.SignUpWithUsernamePassword(username, password);

        if (success) {
            if (statusField != null) statusField.text = "Sign up successful!";
        } else {
            if (statusField != null) statusField.text = "Sign up failed. Username may already exist.";
        }
    }

    public async void IncreaseScore() {
        if (enabled) {
            var username = usernameInputField.text;
            SetScore(scores[username] + 1);
            await DatabaseManager.SaveData(("scores", scores));
        }
    }
}
