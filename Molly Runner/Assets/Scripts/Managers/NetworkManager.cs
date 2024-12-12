
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Android;
using System.Collections;
using OneSignalSDK;
using Firebase;
using TMPro;
using AudienceNetwork;


public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    #region Ads ID

#if UNITY_IOS
    private string interAdID = "0e9c7848ed8b8cb7";
    private string bannerinterAdID = "315e5382b5951515";
    private string rewardAdID = "dadcc97eff02d746";
    private int retryAttempt;
#endif

#if UNITY_ANDROID
    private string interAdID = "ef790220d7cd0037";
    private string bannerinterAdID = "8971189d54e7216e";
    private string rewardAdID = "1c6e45398c04d46d";
    private int retryAttempt;
#endif

    #endregion
    public string sessionPrefToken;
    public bool isNetworkSuccess = false;

    public int networkMyGolds;
    [SerializeField] private GameObject errorTab;
    [SerializeField] private TextMeshProUGUI errorText;
    [HideInInspector] public bool noAdsBought = false;


    void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);

        // SDK Initialialization Appstart
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
        };

        // Initialize MAX SDK
        MaxSdk.SetSdkKey("5SlJ6aP1Gg4iXiiwRtnZ1G_R_S4uSQ087x5psnpPtG0hscClGaQhQxcd7W6Q-oEsRibVvakEucz3tQt__4egX9");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

#if UNITY_ANDROID
        AdSettings.SetDataProcessingOptions(new string[] { });
#endif

    }
    void Start()
    {

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {

#if UNITY_IOS || UNITY_IPHONE
            if (MaxSdkUtils.CompareVersions(UnityEngine.iOS.Device.systemVersion, "14.5") != MaxSdkUtils.VersionComparisonResult.Lesser)
            {
                // Note that App transparency tracking authorization can be checked via `sdkConfiguration.AppTrackingStatus` for Unity Editor and iOS targets
                // 1. Set Meta ATE flag here, THEN
            }
#endif

            // 2. Load ads
        };

        OneSignalInitialize();
        FirebaseInitialize();

        //    InitializeAds();
        InitializeInterstitialAds();
        InitializeRewardedAds();
        InitializeBannerAds();

        noAdsBought = SaveManager.instance.noAdsOwned;


    }


    public void GetUserSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<GetUser>(data);
        SaveManager.instance.currentLevel = response.data.user.level;
        SaveManager.instance.startUnitsLevel = response.data.user.molly_count;
        SaveManager.instance.incomeLevel = response.data.user.income_level;
    }
    public void GetUserFailCallback(string data)
    {
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("Connection Lost!\n Check your connection \n &\n restart the game"));
    }

    #region OneSignal & Firebase
    void FirebaseInitialize()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                var app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                // UnityEngine.Debug.LogError(System.String.Format(
                //   "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    void OneSignalInitialize()
    {
        int myID = UnityEngine.Random.Range(0, 1000000000);
        string externalUserId = myID.ToString(); // You will supply the external user id to the OneSignal SDK
        OneSignal.Default.Initialize("fd22e728-adc3-4929-a1f1-79cdf8317e83");
        OneSignal.Default.SetExternalUserId(externalUserId);
    }

    #endregion

    #region Banner Ads

    //Retrieve the ID from your account

    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerinterAdID, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerinterAdID, new Color(0.2039216f, 0.0509804f, 0.3686275f, 1f));

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

    }

    private void OnBannerAdLoadedEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string interAdID, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    // TO DO: call
    public void ShowBanner()
    {
        if (noAdsBought == false && MaxSdk.IsInitialized())
        {
            return;
        }
        MaxSdk.ShowBanner(bannerinterAdID);

    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerinterAdID);
    }


    #endregion

    #region Interstitials


    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interAdID);
    }

    private void OnInterstitialLoadedEvent(string interAdID, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(interAdID) now returns 'true'
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string interAdID, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string interAdID, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.

        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string interAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string interAdID, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.

        LoadInterstitial();
    }

    //TO DO: Call
    void ShowInterstitial()
    {
        if (noAdsBought == false)
        {
            return;
            if (MaxSdk.IsInterstitialReady(interAdID))
            {
                MaxSdk.ShowInterstitial(interAdID);
            }
        }

    }

    #endregion

    #region Rewarded
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardAdID);
    }

    public void OnRewardedAdLoadedEvent(string rewardAdID, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt = 0;
    }

    public void OnRewardedAdLoadFailedEvent(string rewardAdID, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string rewardAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string rewardAdID, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string rewardAdID, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string rewardAdID, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    public void OnRewardedAdReceivedRewardEvent(string rewardAdID, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        RewardGet(FailCallback, GetRewardSuccessCallback);

    }

    private void OnRewardedAdRevenuePaidEvent(string rewardAdID, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        if (GameManager.instance.gameObject != null)
            RewardAdd(GameManager.instance.RewardAddSuccessCallback, FailCallback, 125, SaveManager.instance.currentLevel, false, false, noAdsBought);

    }
    public void ShowRewarded()
    {

        if (MaxSdk.IsRewardedAdReady(rewardAdID))
        {
            return;
            MaxSdk.ShowRewardedAd(rewardAdID);

        }
    }
    #endregion

    #region Backend
    public delegate void NetworkCallback(string text);
    private string BaseURL = "https://uat-runner-api.minego.co/api-v1";



    // Her girişte çalıştır acces token ı çek
    public void AuthRegister(NetworkCallback failCallback, NetworkCallback successCallback)
    {

        WWWForm formData = new WWWForm();
        //   formData.AddField("username", SplashManager1.instance.usernameIF.text);
        formData.AddField("username", AuthenticationManager.instance.registerUsernameIF.text);
        formData.AddField("password", AuthenticationManager.instance.registerPasswordIF.text);
        formData.AddField("passwordConfirm", AuthenticationManager.instance.registerPasswordConfirmIF.text);
        formData.AddField("device_id", SystemInfo.deviceUniqueIdentifier.ToString());

        StartCoroutine(_PostRequest(BaseURL + "/auth/register", formData, failCallback, successCallback));
    }

    public void AuthLogin(NetworkCallback failCallback, NetworkCallback successCallback, string username, string password)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("username", username);
        formData.AddField("password", password);
        formData.AddField("device_id", SystemInfo.deviceUniqueIdentifier.ToString());

        AuthenticationManager.instance.loginUsernameIF.text = username;
        AuthenticationManager.instance.loginPasswordIF.text = password;

        StartCoroutine(_PostRequest(BaseURL + "/auth/login", formData, failCallback, successCallback));
    }


    public void GetUser(NetworkCallback failCallback, NetworkCallback successCallback)
    {
        WWWForm formData = new WWWForm();
        //   formData.AddField("username", SplashManager1.instance.usernameIF.text);
        formData.AddField("access_token", sessionPrefToken.ToString());
        StartCoroutine(_PostRequest(BaseURL + "/auth/getUser", formData, failCallback, successCallback));
    }

    public void RewardAdd(NetworkCallback failCallback, NetworkCallback successCallback, int moly_count, int level, bool hasBoss, bool hasBossReward, bool hasAds)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        formData.AddField("moly_count", moly_count);
        formData.AddField("level", level);
        formData.AddField("hasBoss", hasBoss.ToString().ToLower());
        formData.AddField("hasBossReward", hasBossReward.ToString().ToLower());
        formData.AddField("hasAds", hasAds.ToString().ToLower());

        StartCoroutine(_PostRequest(BaseURL + "/reward/add", formData, failCallback, successCallback));
    }

    public void RewardGet(NetworkCallback failCallback, NetworkCallback successCallback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        StartCoroutine(_PostRequest(BaseURL + "/reward/get", formData, failCallback, successCallback));
    }

    public void IncreaseMolly(NetworkCallback failCallback, NetworkCallback successCallback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        StartCoroutine(_PostRequest(BaseURL + "/reward/increaseMolly", formData, failCallback, successCallback));
    }
    public void UpgradeMolly(NetworkCallback failCallback, NetworkCallback successCallback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        StartCoroutine(_PostRequest(BaseURL + "/reward/upgrade", formData, failCallback, successCallback));
    }

    public void ItemAddForUser(NetworkCallback failCallback, NetworkCallback successCallback, int item_id)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        formData.AddField("item_id", item_id);
        //  formData.AddField("item_id", item_id);
        StartCoroutine(_PostRequest(BaseURL + "/item/addForUser", formData, failCallback, successCallback));
    }

    public void ItemGetForUser(NetworkCallback failCallback, NetworkCallback successCallback)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        StartCoroutine(_PostRequest(BaseURL + "/item/getUserItems", formData, failCallback, successCallback));
    }

    public void Leaderboard(NetworkCallback failCallback, NetworkCallback successCallback, int perPage, int page)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("access_token", sessionPrefToken.ToString());
        formData.AddField("perPage", perPage);
        formData.AddField("page", page);
        StartCoroutine(_PostRequest(BaseURL + "/reward/leaderboard", formData, failCallback, successCallback));
    }
    IEnumerator _GetRequest(string BaseURL, NetworkCallback FailCallback, NetworkCallback SuccessCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BaseURL))
        {

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = BaseURL.Split('/');
            int page = pages.Length - 1;

            var error = webRequest.error;
            var response = webRequest.downloadHandler.text;
            var webRequestResult = webRequest.result;
            print(webRequestResult);
            webRequest.Dispose();


            switch (webRequestResult)
            {
                case UnityWebRequest.Result.ConnectionError:
                    StartCoroutine(ErrorPanel("Connection Lost!\n Check your connection \n &\n restart the game"));
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    FailCallback(response);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    FailCallback(response);
                    break;
                case UnityWebRequest.Result.Success:
                    print(response);
                    print(pages[page]);
                    SuccessCallback?.Invoke(response);
                    break;
            }
        }
    }

    IEnumerator _PostRequest(string BaseURL, WWWForm formData, NetworkCallback FailCallback, NetworkCallback SuccessCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(BaseURL, formData))
        {
            webRequest.SetRequestHeader("AUTHORIZATION", sessionPrefToken);
            yield return webRequest.SendWebRequest();

            string[] pages = BaseURL.Split('/');
            int page = pages.Length - 1;
            var error = webRequest.error;
            var response = webRequest.downloadHandler.text;
            var webRequestResult = webRequest.result;
            print(webRequestResult);
            FailCallbackError message = JsonUtility.FromJson<FailCallbackError>(response);
            webRequest.Dispose();

            switch (webRequestResult)
            {
                case UnityWebRequest.Result.ConnectionError:
                    StartCoroutine(ErrorPanel("Connection Lost!\n Check your connection \n &\n restart the game"));
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    StartCoroutine(ErrorPanel(message.msg));
                    FailCallback?.Invoke(response);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    StartCoroutine(ErrorPanel(message.msg));
                    FailCallback?.Invoke(response);
                    break;
                case UnityWebRequest.Result.Success:
                    SuccessCallback?.Invoke(response);
                    break;
            }
        }
    }

    #endregion

    #region user

    public void FailCallback(string data)
    {
        var response = JsonUtility.FromJson<FailCallbackError>(data);
        print(data);
        isNetworkSuccess = response.success;
    }



    public void RegisterSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<Register>(data);

        sessionPrefToken = response.data.user.access_token;
        isNetworkSuccess = response.success;


        print(data);
        errorTab.SetActive(false);

        AuthLogin(FailCallback, LoginSuccessCallback, AuthenticationManager.instance.registerUsernameIF.text, AuthenticationManager.instance.registerPasswordIF.text);
    }

    public void LoginSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<Login>(data);
        AuthenticationManager.instance.welcomeBackPanel.SetActive(true);
        AuthenticationManager.instance.welcomeBackText.text = "Welcome back! \n " + PlayerPrefs.GetString("username");

        sessionPrefToken = response.data.access_token;
        isNetworkSuccess = response.success;

        RewardGet(FailCallback, GetRewardSuccessCallback);
        print(data);

        errorTab.SetActive(false);

        PlayerPrefs.SetString("username", AuthenticationManager.instance.loginUsernameIF.text);
        PlayerPrefs.SetString("password", AuthenticationManager.instance.loginPasswordIF.text);

    }

    public void GetRewardSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<GetReward>(data);
        networkMyGolds = response.data.coin;
        isNetworkSuccess = response.success;
        print(data);
        if (LoadManager.instance.gameObject != null)
        {
            LoadManager.instance.splashScreen.SetActive(true);
            LoadManager.instance.signScreen.SetActive(false);
            SplashManager1.instance.CheckNetwork();
        }
    }
    #endregion

    private int connectionLostAttempt = 0;

    public IEnumerator ErrorPanel(string error)
    {
        if (error == "Connection Lost!\n Check your connection \n &\n restart the game" && errorTab.gameObject != null)
        {
            errorTab.gameObject.SetActive(true);
            errorTab.GetComponent<Animation>().Play();
            errorText.text = error;
            connectionLostAttempt++;
            if (connectionLostAttempt > 10)
            {
                Application.Quit();
            }
        }
        else
        {
            errorTab.gameObject.SetActive(true);
            errorTab.GetComponent<Animation>().Play();
            errorText.text = error;
            yield return new WaitForSeconds(.33f + 1f);
            errorTab.GetComponent<Animation>().Play("ReverseErrorTabAnim");
            yield return new WaitForSeconds(.33f);
            errorTab.gameObject.SetActive(false);
        }

    }

}
