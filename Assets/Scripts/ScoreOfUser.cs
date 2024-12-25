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
public class ScoreOfUser: MonoBehaviour {
    [SerializeField] TMP_Text scoreField;
    [SerializeField] TMP_InputField usernameField;

    string username;
    Dictionary<string, object> userdata = null;
    int score = -1;
    private void Awake() {
        enabled = false;
    }

    public async void Initialize() {
        if (!AuthenticationService.Instance.IsSignedIn)
            return;
        username = usernameField.text;
        //Debug.Log($"username={username} len={username.Length}");

        await LoadUserData();
        if (userdata.ContainsKey("score")) {
            //Debug.Log("userdata[score] is of type " + userdata["score"].GetType().Name);
            score = Convert.ToInt32(userdata["score"]);
        } else { 
            score = 0;
        }
        scoreField.text = "Score: " + score;

        gameObject.SetActive(true);
        enabled = true;
    }

    void SetScore(int newscore) {
        userdata["score"] = score = newscore;
        scoreField.text = "Score: " + newscore;
    }

    public async Task LoadUserData() {
        //var playerData = await DatabaseManager.LoadData(username);
        if (!AuthenticationService.Instance.IsSignedIn) return;
        Debug.Log("LoadUserData for username=" + username);
        var keys = new HashSet<string> { username };
        //Debug.Log("keys=" + string.Join(",",keys));
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        if (playerData.TryGetValue(username, out var userdataItem)) {
            userdata = userdataItem.Value.GetAs<Dictionary<string,object>>();
            Debug.Log($"loaded user data: {userdata}");
        } else {
            userdata = new Dictionary<string, object>();
            Debug.Log($"no user data - initializing to empty dictionary");
        }
    }

    public async Task SaveUserData() {
        await DatabaseManager.SaveData((username, userdata));
    }

    public async void IncreaseScore() {
        if (enabled) {
            SetScore(score + 1);
            await SaveUserData();
        }
    }

}
