using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    #region panels

    //Screen object variables
    public GameObject inGamePanel;
    public GameObject gameStartPanel;
    public GameObject finishPanel;
    public GameObject marketplacePanel;
    public GameObject walletPanel;
    public GameObject settingsPanel;
    public GameObject failedPanel;
    public GameObject VFXMarketplacePanel;
    public GameObject leaderboardPanel;
    public GameObject faqPanel;
    public GameObject bundlePanel;



    #endregion


    #region gamestartpanel

    public Button marketplaceButton;
    public Button settingsButton;
    public Button vfxMarketButton;
    public Button faqButton;


    #endregion

    public Button tryAgainButton;
    public Button reloadLevelButton;
    public Button walletButton;

    private GameObject lastObject;

    public Transform errorPanel;
    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()

    {
        instance = this;
    }

    void Start()
    {
        gameStartPanel.SetActive(true);
        marketplaceButton.onClick.AddListener(MarketplaceScreen);
        //   walletButton.onClick.AddListener(WalletScreen);
        settingsButton.onClick.AddListener(SettingsScreen);
        tryAgainButton.onClick.AddListener(TryAgain);
        vfxMarketButton.onClick.AddListener(VFXMarketplaceScreen);
        walletButton.onClick.AddListener(ComingSoon);
        reloadLevelButton.onClick.AddListener(TryAgain);
        faqButton.onClick.AddListener(FaqScreen);
    }

    //Functions to change the login screen UI
    public void ClearScreen() //Turn off all screens
    {
        inGamePanel.SetActive(false);
        gameStartPanel.SetActive(false);
        finishPanel.SetActive(false);
        walletPanel.SetActive(false);
        settingsPanel.SetActive(false);
        failedPanel.SetActive(false);
        VFXMarketplacePanel.SetActive(false);
        marketplacePanel.SetActive(false);
        leaderboardPanel.SetActive(false);
        faqPanel.SetActive(false);
        bundlePanel.SetActive(false);

        if (marketplacePanel.activeSelf)
            Marketplace.instance.StartCoroutine(Marketplace.instance.CloseButton());
    }

    public void FaqScreen()
    {
        ClearScreen();
        faqPanel.SetActive(true);
        gameStartPanel.SetActive(true);
        NetworkManager.instance.HideBanner();
    }

    public void MarketplaceScreen() // Marketplace: cannot be reachable while playing
    {
        ClearScreen();
        marketplacePanel.SetActive(true);
        NetworkManager.instance.ItemGetForUser(Marketplace.instance.GetUserFailCallback, Marketplace.instance.ItemGetForUserSuccessCallback);

        if (GameManager.instance.gameObject != null)
            NetworkManager.instance.RewardGet(GameManager.instance.FailCallback, GameManager.instance.GetRewardSuccessCallback);

        NetworkManager.instance.HideBanner();

    }

    public void BundlePanel()
    {
        ClearScreen();
        bundlePanel.SetActive(true);
        NetworkManager.instance.HideBanner();
    }

    public void LeaderboardScreen()
    {
        ClearScreen();
        leaderboardPanel.SetActive(true);
        lastObject = gameStartPanel;
        NetworkManager.instance.HideBanner();
    }
    public void InGameScreen()  // Playing
    {
        ClearScreen();
        inGamePanel.SetActive(true);
        lastObject = inGamePanel;
        PlayerController.instance.babiesParent.GetChild(0).gameObject.SetActive(true);
        NetworkManager.instance.ShowBanner();

    }
    public void GameStartScreen()   // Start idle
    {
        ClearScreen();
        gameStartPanel.SetActive(true);
        lastObject = gameStartPanel;
        PlayerController.instance.babiesParent.GetChild(0).gameObject.SetActive(false);
        NetworkManager.instance.ShowBanner();

    }
    public void FinishScreen()  // finished
    {
        ClearScreen();
        finishPanel.SetActive(true);
    }
    public void WalletScreen()  // wallet
    {
        ClearScreen();
        walletPanel.SetActive(true);
        NetworkManager.instance.HideBanner();


    }
    public void SettingsScreen()  // settings
    {
        ClearScreen();
        settingsPanel.SetActive(true);
    }
    public void BackButton()
    {

        ClearScreen();
        lastObject.SetActive(true);
        Marketplace.instance.buyTab.gameObject.SetActive(false);
        NetworkManager.instance.ShowBanner();

    }

    public void FailedScreen()
    {
        ClearScreen();
        failedPanel.SetActive(true);
    }

    void TryAgain()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }

    void VFXMarketplaceScreen()
    {
        ClearScreen();
        VFXMarketplacePanel.gameObject.SetActive(true);
        NetworkManager.instance.HideBanner();

    }

    public IEnumerator ErrorPanel(string error)
    {
        errorPanel.gameObject.SetActive(true);
        errorPanel.GetComponent<Animation>().Play();
        errorText.text = error;
        yield return new WaitForSeconds(errorPanel.GetComponent<Animation>().GetClip("ErrorTabAnim").length + 1f);
        errorPanel.GetComponent<Animation>().Play("ReverseErrorTabAnim");
        yield return new WaitForSeconds(errorPanel.GetComponent<Animation>().GetClip("ReverseErrorTabAnim").length);
        errorPanel.gameObject.SetActive(false);
    }

    public void ComingSoon()
    {
        StartCoroutine(ErrorPanel("Coming Soon!"));
    }
}