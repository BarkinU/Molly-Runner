using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{

    public static Leaderboard instance;
    public List<GameObject> firstPageleaderboardItems = new List<GameObject>();
    public List<GameObject> secondPageleaderboardItems = new List<GameObject>();
    public List<GameObject> thirdleaderboardItems = new List<GameObject>();
    public List<GameObject> fourthPageleaderboardItems = new List<GameObject>();

    [SerializeField] private Button firstPage;
    [SerializeField] private Button secondPage;
    [SerializeField] private Button thirdPage;
    [SerializeField] private Button fourthPage;

    [SerializeField] private GameObject firstPageLeaderboard;
    [SerializeField] private GameObject secondPageLeaderboard;
    [SerializeField] private GameObject thirdPageLeaderboard;
    [SerializeField] private GameObject fourthPageLeaderboard;


    private int perPage = 25;
    private int page = 1;

    private void Start()
    {
        firstPage.onClick.AddListener(FirstPage);
        secondPage.onClick.AddListener(SecondPage);
        thirdPage.onClick.AddListener(ThirdPage);
        fourthPage.onClick.AddListener(FourthPage);
    }

    private void OnEnable()
    {
        FirstPage();
    }

    private void ClearScreen()
    {
        firstPageLeaderboard.SetActive(false);
        secondPageLeaderboard.SetActive(false);
        thirdPageLeaderboard.SetActive(false);
        fourthPageLeaderboard.SetActive(false);
    }

    private void FirstPage()
    {
        ClearScreen();
        firstPageLeaderboard.SetActive(true);
        page = 1;
        NetworkManager.instance.Leaderboard(NetworkManager.instance.FailCallback, LeaderboardSuccessCallback, perPage, page);

    }

    private void SecondPage()
    {
        ClearScreen();
        secondPageLeaderboard.SetActive(true);
        page = 2;
        NetworkManager.instance.Leaderboard(NetworkManager.instance.FailCallback, LeaderboardSuccessCallback, perPage, page);
    }

    private void ThirdPage()
    {
        ClearScreen();
        thirdPageLeaderboard.SetActive(true);
        page = 3;
        NetworkManager.instance.Leaderboard(NetworkManager.instance.FailCallback, LeaderboardSuccessCallback, perPage, page);
    }

    private void FourthPage()
    {
        ClearScreen();
        fourthPageLeaderboard.SetActive(true);
        page = 4;
        NetworkManager.instance.Leaderboard(NetworkManager.instance.FailCallback, LeaderboardSuccessCallback, perPage, page);
    }

    private void LeaderboardSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<LeaderboardResponse>(data);
        if (page == 1)
        {
            for (int i = 0; i < response.data.Count; i++)
            {
                firstPageleaderboardItems[i].GetComponent<SetUpLeaderboardItem>().SetUp(response.data[i].username.ToString(), response.data[i].points);
            }
        }
        else if (page == 2)
        {
            for (int i = 0; i < response.data.Count; i++)
            {
                secondPageleaderboardItems[i].GetComponent<SetUpLeaderboardItem>().SetUp(response.data[i].username.ToString(), response.data[i].points);
            }
        }
        else if (page == 3)
        {
            for (int i = 0; i < response.data.Count; i++)
            {
                thirdleaderboardItems[i].GetComponent<SetUpLeaderboardItem>().SetUp(response.data[i].username.ToString(), response.data[i].points);
            }
        }
        else if (page == 4)
        {
            for (int i = 0; i < response.data.Count; i++)
            {
                fourthPageleaderboardItems[i].GetComponent<SetUpLeaderboardItem>().SetUp(response.data[i].username.ToString(), response.data[i].points);
            }
        }
        else
        {
            Debug.LogWarning("There is no such a page");
        }

        // get all pages and set up the leaderboard
    }
}
