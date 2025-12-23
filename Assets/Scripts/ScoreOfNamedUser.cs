using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;

/**
 * Saves the score of a single user
 */
public class ScoreOfNamedUser: MonoBehaviour {
    [SerializeField] TMP_Text scoreField;
    [SerializeField] TMP_InputField usernameField;

    string username;
    Dictionary<string, object> userdata = null;
    int score = -1; // not initialized

    private void Start() {
        enabled = false;
    }

    public async void Initialize() {  // This is NOT called at sign-in - it is called after the user clicks "submit"
        if (!AuthenticationService.Instance.IsSignedIn) {
            Debug.LogError("Not signed in to AuthenticationService!");
            return;
        }

        username = usernameField.text;
        //Debug.Log($"username={username} len={username.Length}");

        userdata = await LoadUserData(username);
        int loadedScore;
        if (userdata.ContainsKey("score")) {
            //Debug.Log("userdata[score] is of type " + userdata["score"].GetType().Name);
            loadedScore = Convert.ToInt32(userdata["score"]);
        } else {
            loadedScore = 0;
        }
        SetScore(loadedScore);
        gameObject.SetActive(true);
        enabled = true;
    }

    void SetScore(int newscore) {
        userdata["score"] = score = newscore;
        scoreField.text = "Score: " + newscore;
    }

    public async Task<Dictionary<string, object>> LoadUserData(string username) {
        var playerData = await DatabaseManager.LoadData(username);

        if (playerData.TryGetValue(username, out var userdataItem)) {
            var userdata = userdataItem.Value.GetAs<Dictionary<string,object>>();
            Debug.Log($"loaded user data: {userdata}");
            return userdata;
        } else {
            var userdata = new Dictionary<string, object>();
            Debug.Log($"no user data - initializing to empty dictionary");
            return userdata;
        }
    }

    public async void IncreaseScore() {
        if (enabled) {
            SetScore(score + 1);
            await DatabaseManager.SaveData((username, userdata));
        }
    }

}
