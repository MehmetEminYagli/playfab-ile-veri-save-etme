using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour
{
    private int score = 0;

    [SerializeField] TextMeshProUGUI Skor;



    [Header("Windows")]
    public GameObject nameWindow;
    public GameObject LeaderBoardWindow;

    [Header("Display Name window")]
    public GameObject nameError;
    public InputField nameInput;

    [Header("LeaderBoaard")]
    public GameObject rowPrefab;
    public Transform rowsParent;
    void Start()
    {
        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request , OnSuccess , OnError);
    }

    void OnSuccess(LoginResult result) {
        Debug.Log("Successful login/account create");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name= result.InfoResultPayload.PlayerProfile.DisplayName;
        
            if (name == null)
            {
                nameWindow.SetActive(true);
            }else
            LeaderBoardWindow.SetActive(true);

        }


    }

    public void SubmitBtn(){
        var request = new UpdateUserTitleDisplayNameRequest{
            DisplayName = nameInput.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request , onDislplayNameUpdate, OnError);
        nameWindow.SetActive(false);
    }

    void onDislplayNameUpdate(UpdateUserTitleDisplayNameResult result) {
        Debug.Log("Updated display name!");
        LeaderBoardWindow.SetActive(true);
    }


    void OnError(PlayFabError error) {
        Debug.Log("erorr");
        Debug.Log(error.GenerateErrorReport());
    }
  
    public void SendleaderBoard(int score) 
    {
        var request = new UpdatePlayerStatisticsRequest{
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request , OnLeaderBoardUpdate , OnError);
    }

    void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result) {
        Debug.Log("Succesfull leaderBoard Sent");
    }


    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount =10
        };
        PlayFabClientAPI.GetLeaderboard(request , OnleaderBoardGet , OnError);
    }
    void OnleaderBoardGet(GetLeaderboardResult result) {
        
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }
        
        foreach(var item in result.Leaderboard) {

            GameObject newGo = Instantiate(rowPrefab ,rowsParent);
            Text[] texts = newGo.GetComponentsInChildren<Text>();
            texts[0].text = (item.Position + 1 ).ToString(); //sıra
            texts[1].text = item.DisplayName; // adı
            texts[2].text = item.StatValue.ToString(); //skoru

            Debug.Log(item.Position + " " + item.PlayFabId+ " " + item.StatValue);
        }
    }

    public void GameOverBtn(){
        SendleaderBoard(score);
    }

    public void GetLeaderBoardBtn() {
        GetLeaderBoard();
    }


  public void plusBtn() 
  {
    score++;
    Skor.text = score.ToString();
  }
   public void minusBtn() 
  {
    score--;
    Skor.text = score.ToString();
  }

}