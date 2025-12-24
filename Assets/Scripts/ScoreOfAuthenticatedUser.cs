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
        Debug.Log("ScoreOfAuthenticatedUser Start");

        // Show sign-in panel, hide game panel initially
        //if (signInPanel != null) signInPanel.SetActive(true);
        //if (gamePanel != null) gamePanel.SetActive(false);

        //// Subscribe to SignedIn event - must be done before sign-in happens
        //AuthenticationService.Instance.SignedIn += Initialize;
        //Debug.Log("Subscribed to SignedIn event");
    }

    void Initialize() {
        Debug.Log("ScoreOfAuthenticatedUser Initialize");
        // Hide sign-in panel, show game panel
        if (signInPanel != null) signInPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);

        // Initialize the score manager with the current username
        scoreManager = GetComponent<ScoreOfNamedUser>();
        if (!scoreManager) {
            Debug.LogError("scoreManager is null!");
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
        statusField.text = signInMessage;//.Substring(0,40)+"...";

        // Manually trigger initialization if sign-in was successful
        // This is a fallback in case the SignedIn event doesn't fire
        if (AuthenticationService.Instance.IsSignedIn) {
            Debug.Log("Manual initialization after sign-in");
            Initialize();
        } else {
            AuthenticationService.Instance.SignedIn += Initialize;
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
        string message = await authManager.SignUpWithUsernamePassword(username, password);

        if (message.Contains("success")) {
            if (statusField != null) statusField.text = "Sign up successful!";
            // Manually trigger initialization after successful sign-up
            // This is a fallback in case the SignedIn event doesn't fire
            Debug.Log(message);
            if (AuthenticationService.Instance.IsSignedIn) {
                Debug.Log("Manual initialization after sign-up");
                Initialize();
            }
        } else {
            if (statusField != null) statusField.text = message;
            Debug.LogError(message);
        }
    }
}
