using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

/**
 * Handles Unity Gaming Services initialization and username+password authentication
 */
public class AuthenticationManagerWithPassword : MonoBehaviour {

    // Initializing the Unity Services SDK
    async void Awake() {
        await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn) {
            Debug.Log($"Player is already signed in as: {AuthenticationService.Instance.PlayerId}");
        } else {
            Debug.Log("Player is not signed in yet - waiting for sign-in");
        }
    }

    /**
     * Sign up a new user with username and password
     */
    public async Task<bool> SignUpWithUsernamePassword(string username, string password) {
        try {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log($"SignUp successful! Player ID: {AuthenticationService.Instance.PlayerId}");
            return true;
        } catch (AuthenticationException ex) {
            Debug.LogError($"SignUp failed: {ex.Message}");
            return false;
        } catch (RequestFailedException ex) {
            Debug.LogError($"SignUp request failed: {ex.Message}");
            return false;
        }
    }

    /**
     * Sign in an existing user with username and password
     */
    public async Task<bool> SignInWithUsernamePassword(string username, string password) {
        try {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log($"SignIn successful! Player ID: {AuthenticationService.Instance.PlayerId}");
            return true;
        } catch (AuthenticationException ex) {
            Debug.LogError($"SignIn failed: {ex.Message}");
            return false;
        } catch (RequestFailedException ex) {
            Debug.LogError($"SignIn request failed: {ex.Message}");
            return false;
        }
    }

    /**
     * Sign out the current user
     */
    public void SignOut() {
        AuthenticationService.Instance.SignOut();
        Debug.Log("Player signed out");
    }
}
