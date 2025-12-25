using TMPro;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using UnityEngine;


/**
 * Handles username+password authentication.
 */
public class ScoreOfAuthenticatedUser : MonoBehaviour {
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TextMeshProUGUI statusField;
    [SerializeField] GameObject signInPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] TextMeshProUGUI textField;

    [SerializeField] AuthenticationManagerWithPassword authManager;

    private int score;

    void Awake() {
        // This happens only when the object is activated!
        Debug.Log("ScoreOfAuthenticatedUser Awake");

        //if (signInPanel != null) signInPanel.SetActive(true);
        //if (gamePanel != null) gamePanel.SetActive(false);
    }

    async void Initialize() {
        //if (!AuthenticationService.Instance.IsSignedIn) {
        //    Debug.LogError("Not signed in to AuthenticationService!");
        //    return;
        //}
        Debug.Log("ScoreOfAuthenticatedUser Initialize");
        // Hide sign-in panel, show game panel
        statusField.gameObject.SetActive(false);
        if (signInPanel != null) signInPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        usernameInputField.readOnly = true;
        passwordInputField.readOnly = true;

        string username = usernameInputField.text;
        Debug.Log($"   username='{username}'");

        int loadedScore;
        var scoreData = await DatabaseManager.LoadData("score");
        if (scoreData.TryGetValue("score", out var scoreVar)) {
            loadedScore = scoreVar.Value.GetAs<int>();
            Debug.Log($"loaded score value: {loadedScore}");
        } else {
            loadedScore = 0;
            Debug.Log($"no score value - initializing to {loadedScore}");
        }
        SetScore(loadedScore);
        gameObject.SetActive(true);
        enabled = true;

        await DatabaseManager.SaveData(("username", usernameInputField.text));
        // Unity Cloud shows only PlayerID - not username. For debugging, we write the username manually.
    }

    enum ButtonType { REGISTER, LOGIN };

    private async Task OnButtonClicked(ButtonType b) {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            statusField.text = "Please enter username and password";
            return;
        }
        string message;
        if (b == ButtonType.REGISTER) {
            statusField.text = "Registering...";
            message = await authManager.RegisterWithUsernameAndPassword(username, password);
        } else {
            statusField.text = "Logging in...";
            message = await authManager.LoginWithUsernameAndPassword(username, password);
        }
        statusField.text = message;

        if (message.Contains("success")) {
            Debug.Log(message);
            if (AuthenticationService.Instance.IsSignedIn) {
                Debug.Log("Already signed in: manual initialization");
                Initialize();
            } else {
                Debug.Log("Not signed in yet: Initialize at SignedIn event");
                AuthenticationService.Instance.SignedIn += Initialize;
            }
        } else {
            Debug.LogError(message);
        }
    }

    public async void OnSignInButtonClicked() {
        await OnButtonClicked(ButtonType.LOGIN);
    }

    public async void OnSignUpButtonClicked() {
        await OnButtonClicked(ButtonType.REGISTER);
    }

    void SetScore(int newscore) {
        score = newscore;
        textField.text = "Score: " + score;
    }

    public async void IncreaseScore() {
        Debug.Log($"IncreaseScore: enabled={enabled}");
        if (enabled) {
            SetScore(score + 1);
            await DatabaseManager.SaveData(("score", score));
        }
    }

}
