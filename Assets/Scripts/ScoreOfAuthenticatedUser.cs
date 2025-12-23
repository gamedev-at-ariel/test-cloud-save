using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;


/**
 * Handles username+password authentication and delegates score management to ScoreOfNamedUser
 */
[RequireComponent(typeof(ScoreOfNamedUser))]
public class ScoreOfAuthenticatedUser : MonoBehaviour {
    [SerializeField] TMP_InputField usernameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TextMeshProUGUI statusField;
    [SerializeField] GameObject signInPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] AuthenticationManagerWithPassword authManager;
    
    private ScoreOfNamedUser scoreManager;

    void Start() {
        // Show sign-in panel, hide game panel initially
        if (signInPanel != null) signInPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);

        // Subscribe to SignedIn event - must be done before sign-in happens
        AuthenticationService.Instance.SignedIn += OnSignedIn;
        Debug.Log("Subscribed to SignedIn event");
    }

    void OnSignedIn() {
        Debug.Log("SignedIn event called");
        // Hide sign-in panel, show game panel
        if (signInPanel != null) signInPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);

        // Initialize the score manager with the current username
        scoreManager = GetComponent<ScoreOfNamedUser>();
        if (!scoreManager) {
            Debug.LogError("scoreManager is null in OnSignedIn!");
        } else {
            scoreManager.Initialize();
        }
    }

    public async void OnSignInButtonClicked() {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
            statusField.text = "Please enter username and password";
            return;
        }

        statusField.text = "Signing in...";
        string signInMessage = null;
        if (!authManager)        {
            signInMessage = "authManager is null!";
        } else        {
            signInMessage = await authManager.SignInWithUsernamePassword(username, password);
        }

        Debug.Log(signInMessage);
        statusField.text = signInMessage.Substring(0,40)+"...";

        // Manually trigger initialization if sign-in was successful
        // This is a fallback in case the SignedIn event doesn't fire
        if (AuthenticationService.Instance.IsSignedIn) {
            Debug.Log("Manual initialization after sign-in");
            OnSignedIn();
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
            // Manually trigger initialization after successful sign-up
            // This is a fallback in case the SignedIn event doesn't fire
            if (AuthenticationService.Instance.IsSignedIn) {
                Debug.Log("Manual initialization after sign-up");
                OnSignedIn();
            }
        } else {
            if (statusField != null) statusField.text = "Sign up failed. Username may already exist.";
        }
    }
}
