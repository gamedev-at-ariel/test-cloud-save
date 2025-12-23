using TMPro;
using Unity.Services.Authentication;
using UnityEngine;


/**
 * Handles username+password authentication and delegates score management to ScoreOfNamedUser
 */
public class ScoreOfAuthenticatedUser : MonoBehaviour {
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TextMeshProUGUI statusField;
    [SerializeField] GameObject signInPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] ScoreOfNamedUser scoreManager;

    private AuthenticationManagerWithPassword authManager;

    void Start() {
        authManager = FindAnyObjectByType<AuthenticationManagerWithPassword>();

        // Show sign-in panel, hide game panel initially
        if (signInPanel != null) signInPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);

        AuthenticationService.Instance.SignedIn += () => {
            // Hide sign-in panel, show game panel
            if (signInPanel != null) signInPanel.SetActive(false);
            if (gamePanel != null) gamePanel.SetActive(true);

            // Initialize the score manager with the current username
            scoreManager.Initialize();
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

    public void IncreaseScore() {
        // Delegate to ScoreOfNamedUser
        scoreManager.IncreaseScore();
    }
}
