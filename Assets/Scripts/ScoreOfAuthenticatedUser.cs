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
    private void Awake()
    {
        Debug.Log("ScoreOfAuthenticatedUser Awake");
        scoreManager = GetComponent<ScoreOfNamedUser>();
    }

    void Start() {

        // Show sign-in panel, hide game panel initially
        if (signInPanel != null) signInPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("SignedIn event called");
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
