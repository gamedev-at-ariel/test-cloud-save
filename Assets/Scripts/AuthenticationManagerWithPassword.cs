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
    public async Task<string> SignUpWithUsernamePassword(string username, string password) {
        try {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            return ($"SignUp successful! Player ID: {AuthenticationService.Instance.PlayerId}");
        } catch (AuthenticationException ex) {
            return ($"SignUp failed: {ex.Message}");
        } catch (RequestFailedException ex) {
            return ($"SignUp request failed: {ex.Message}");
        }
    }

    /**
     * Sign in an existing user with username and password
     */
    public async Task<string> SignInWithUsernamePassword(string username, string password) {
        try {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            return $"SignIn successful! Player ID: {AuthenticationService.Instance.PlayerId}";
        } catch (AuthenticationException ex) {
            return $"SignIn failed: {ex.Message}";
        } catch (RequestFailedException ex) {
            return $"SignIn request failed: {ex.Message}";
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
