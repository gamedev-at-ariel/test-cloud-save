using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Authentication;   // AuthenticationService
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;             // UnityServices
using UnityEngine;
using System.Linq;

public class AuthenticationManager: MonoBehaviour {

    // Initializing the Unity Services SDK: https://docs.unity.com/ugs/en-us/manual/authentication/manual/get-started
    async void Awake() {
        await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn) {
            Debug.Log($"Player is already signed in as: {AuthenticationService.Instance.PlayerId}");
        } else {
            Debug.Log("Player is not signed in yet - signing in anonymously");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log($"Signed in as player id: {AuthenticationService.Instance.PlayerId} Access Token: {AuthenticationService.Instance.AccessToken}");
    }

}
