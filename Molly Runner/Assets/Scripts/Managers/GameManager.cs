using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    #region  Mains

    public static GameManager instance;
    public TextMeshProUGUI totalBabyCountText;
    private PlayerController playerController;
    public List<GameObject> totalBabyList = new List<GameObject>();
    public TextMesh finishText; // stand
    [SerializeField] private int levelCount;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button incomeUpgradeButton;
    [SerializeField] private Button increaseMollyButton;


    #endregion

    #region bools
    public bool isStarted;
    private bool gameFinished = false;
    [HideInInspector] public bool canBuy;

    #endregion

    #region save system

    [SerializeField] private float constantReward = 50f;
    [SerializeField] private TextMeshProUGUI myGoldsText;
    [SerializeField] private TextMeshProUGUI walletGoldText;
    [SerializeField] private TextMeshProUGUI marketMyGoldsText;
    [SerializeField] public TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI startUnitsLevelText;
    [SerializeField] private TextMeshProUGUI incomeLevelText;
    [SerializeField] private TextMeshProUGUI startUnitsUpgradeCostText;
    [SerializeField] private TextMeshProUGUI incomeUpgradeCostText;

    #endregion

    #region AttackSystem

    public int playerAttackValue;
    public int playerDefenseValue;
    [SerializeField] private Slider playerAttackBar;
    [SerializeField] private TextMeshProUGUI playerAttackText;
    [SerializeField] private Slider playerDefenseBar;

    [SerializeField] private TextMeshProUGUI playerDefenseText;


    #endregion

    #region LevelSystem 

    [SerializeField] private List<GameObject> levels;

    #endregion

    #region Backend

    public int myGold;

    #endregion

    #region Boss Adjustments

    private float timeScaleConstant = 1f;
    public Button increaseFightSpeedButton;
    [SerializeField] private Animation increaseFightSpeedAnim;
    [SerializeField] private Slider increaseFightSpeedSlider;

    #endregion

    [SerializeField] private bool targetFound;
    public bool hasAds = false;
    [SerializeField] private bool targetDied;
    [SerializeField] private GameObject splashScreenForWait;
    [SerializeField] private Image splashScreenImage;
    [SerializeField] private Image splashScreenImageSecond;
    [SerializeField] private Image splashScreenImageThird;
    [SerializeField] private GameObject noAdsButton;
    [SerializeField] private GameObject restoreButton;

    private void Awake()
    {

        if (instance == null && instance != this)
            instance = this;


    }

    private void OnEnable()
    {
        levelCount = levels.Count;
    }

    private void Start()
    {
        playerController = PlayerController.instance;
        myGold = NetworkManager.instance.networkMyGolds;
        totalBabyList = new List<GameObject>();
        Time.timeScale = 1f;

        NoAdsBoughtAction();
        //Setting up restore button for apple
#if UNITY_ANDROID
        if (restoreButton.gameObject != null)
            restoreButton.gameObject.SetActive(false);
#endif
#if UNITY_IOS
        if (restoreButton.gameObject != null)
        restoreButton.gameObject.SetActive(true);
#endif

        increaseFightSpeedButton.onClick.AddListener(BossIncreaseFightSpeed);
        NetworkManager.instance.RewardGet(FailCallback, GetRewardSuccessCallback);
        NetworkManager.instance.ItemGetForUser(Marketplace.instance.GetUserFailCallback, Marketplace.instance.ItemGetForUserSuccessCallback);
        NetworkManager.instance.GetUser(GetUserFailCallback, GetUserSuccessCallback);
        NetworkManager.instance.ShowBanner();

        Invoke("LoadNetworkValues", 0);
        InvokeRepeating("SplashScreenCloser", 0.1f, .01f);

    }
    private bool startOpDone = false;
    private void SplashScreenCloser()
    {

        splashScreenForWait.gameObject.SetActive(true);
        splashScreenImage.color = Color32.Lerp(splashScreenImage.color, new Color32(0, 0, 0, 0), Time.deltaTime);
        splashScreenImageSecond.color = Color32.Lerp(splashScreenImageSecond.color, new Color32(0, 0, 0, 0), Time.deltaTime);
        splashScreenImageThird.color = Color32.Lerp(splashScreenImageThird.color, new Color32(0, 0, 0, 0), Time.deltaTime);
        if (splashScreenImage.color.a < .9)
        {
            if (startOpDone == false)
            {
                Invoke("StartOperation", .08f);
                startOpDone = true;
            }
        }
        if (splashScreenImage.color.a < .02f)
        {
            splashScreenImage.color = Color.white;
            splashScreenImageSecond.color = Color.white;
            splashScreenImageThird.color = Color.white;
            splashScreenForWait.gameObject.SetActive(false);
            CancelInvoke("SplashScreenCloser");
        }

    }

    private void StartOperation()
    {
        if (SaveManager.instance.startUnitsLevel > 600)
        {
            SaveManager.instance.startUnitsLevel = 599;
        }
        for (int i = 0; i < SaveManager.instance.startUnitsLevel; i++)
        {
            float radiusX = Random.insideUnitCircle.x;
            float radiusZ = Random.insideUnitCircle.y;
            GameObject runnerInstance = ObjectPooler.instance.SpawnFromPool("Baby", playerController.transform.position + new Vector3(radiusX, playerController.transform.position.y, radiusZ) - Vector3.forward * 1.8f, Quaternion.identity);
            runnerInstance.transform.parent = playerController.babiesParent;
            totalBabyList.Add(runnerInstance);
        }

        totalBabyList = GameObject.FindGameObjectsWithTag("Baby").ToList();
        TextAdjustments();
    }

    private void LoadNetworkValues()
    {
        if (NetworkManager.instance.isNetworkSuccess)
        {
            nextLevelButton.onClick.AddListener(NextLevel);
            incomeUpgradeButton.onClick.AddListener(IncomeUpgrade);
            increaseMollyButton.onClick.AddListener(StartUnitsUpgrade);
            myGold = NetworkManager.instance.networkMyGolds;
            myGoldsText.text = myGold.ToString();
            walletGoldText.text = myGoldsText.text;
            marketMyGoldsText.text = myGoldsText.text;
            currentLevelText.text = "Level" + SaveManager.instance.currentLevel;
            if (SaveManager.instance.currentLevel == 100)
            {
                Instantiate(levels[levels.Count - 1]);
            }
            else
                Instantiate(levels[(SaveManager.instance.currentLevel % 100) - 1]);

            splashScreenImage.color = new Color32(255, 255, 255, 255);
            splashScreenForWait.gameObject.SetActive(true);

            TextAdjustments();
        }
    }

    private void FixedUpdate()
    {
        if (isStarted && playerController.currentPhase == PlayerController.Phases.inGame)
        {
            BabyMoveToLocation();
        }

        totalBabyCountText.text = totalBabyList.Count.ToString();

    }

    public void FinishEvent()
    {
        if (gameFinished == false)
        {
            UIManager.instance.FinishScreen();
            Vector3 firstPos = finishText.transform.parent.localPosition;
            finishText.transform.parent.localPosition = firstPos + Vector3.right * 3 - Vector3.forward * 2;
            finishText.transform.parent.GetComponent<Animation>().Play();

            if (playerController.target == null)
            {
                finishText.text = "Level Completed! \n" + "You saved \n" + GameManager.instance.totalBabyList.Count.ToString() + " Baby Molly";
            }
            else
            {
                finishText.text = "Level Completed! \n" + "You got " + constantReward.ToString() + " \n " + " Gold";
            }
            gameFinished = true;

        }
    }

    public IEnumerator OnPlayClick()
    {
        isStarted = true;
        UIManager.instance.InGameScreen();
        playerController.myAnim.SetBool("isRunning", true);

        SaveManager.instance.Save();

        if (playerController.target != null)
            targetFound = true;
        else
            targetFound = false;
        AttackAdjustment();

        if (GameObject.FindGameObjectWithTag("Enemy"))
            playerController.target = GameObject.FindGameObjectWithTag("Enemy").transform;

        playerController.crowdSound.Play();
        if (GameObject.FindGameObjectWithTag("Enemy"))
        {
            Enemy enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
            int i = ((int)(SaveManager.instance.startUnitsLevel * .3f));
            enemy.health += i;
            enemy.healthBar.maxValue = enemy.health;
            enemy.healthBar.value = enemy.health;
            enemy.enemyHealthBarText.text = enemy.health.ToString();

        }
        foreach (GameObject item in totalBabyList)
        {
            item.GetComponent<BabyController>().OnPlayClickBaby();
        }
        yield return null;
    }
    public void IncomeUpgrade()
    {
        NetworkManager.instance.UpgradeMolly(FailCallback, UpgradeIncomeSuccessCallback);
    }


    public void StartUnitsUpgrade()
    {
        NetworkManager.instance.IncreaseMolly(FailCallback, IncreaseMollySuccessCallback);
    }

    public void NextLevel()
    {
        if (SaveManager.instance.currentLevel % 5 == 0)
        {
            if (playerController.target.GetComponent<Enemy>().isDead)
                targetDied = true;
            else
                targetDied = false;
        }

        SaveManager.instance.currentLevel += 1;

        SaveManager.instance.currentLevel = Mathf.Clamp(SaveManager.instance.currentLevel, 1, 500);
        NetworkManager.instance.RewardAdd(NetworkManager.instance.FailCallback, RewardAddSuccessCallback, totalBabyList.Count, SaveManager.instance.currentLevel, targetFound, targetDied, hasAds);
        SaveManager.instance.Save();
        Invoke("LoadScene", .2f);



    }

    public void GetUserSuccessCallback(string data)
    {

        var response = JsonUtility.FromJson<GetUser>(data);
        SaveManager.instance.currentLevel = response.data.user.level;
        SaveManager.instance.startUnitsLevel = response.data.user.molly_count;
        SaveManager.instance.incomeLevel = response.data.user.income_level;
        SaveManager.instance.currentLevel = Mathf.Clamp(SaveManager.instance.currentLevel, 1, 500);

        TextAdjustments();
        SaveManager.instance.Save();


    }

    private void UpgradeIncomeSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<IncreaseMolly>(data);
        SaveManager.instance.incomeLevel = response.level;
        SaveManager.instance.incomeUpgradeCost = myGold - response.totalCoin;
        myGold = response.totalCoin;

        AudioManager.instance.Stop("coin1");
        AudioManager.instance.Play("coin1");

        canBuy = false;
        NetworkManager.instance.GetUser(GetUserFailCallback, GetUserSuccessCallback);

        TextAdjustments();
        SaveManager.instance.Save();
    }

    private void IncreaseMollySuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<UpgradeMolly>(data);

        SaveManager.instance.startUnitsLevel = response.molly_count;
        SaveManager.instance.startUnitsUpgradeCost = myGold - response.totalCoin;    // sana cost döndürmeli yoksa değişir

        myGold = response.totalCoin;

        AudioManager.instance.Play("BabyAddition");
        float radiusX = Random.insideUnitCircle.x;
        float radiusZ = Random.insideUnitCircle.y;
        GameObject runnerInstance = ObjectPooler.instance.SpawnFromPool("Baby", playerController.transform.position + new Vector3(radiusX, playerController.transform.position.y, radiusZ) - Vector3.forward * 1.8f, Quaternion.identity);
        runnerInstance.transform.parent = playerController.babiesParent;

        totalBabyList.Add(runnerInstance);

        NetworkManager.instance.GetUser(GetUserFailCallback, GetUserSuccessCallback);
        canBuy = false;
        NetworkManager.instance.networkMyGolds = myGold;
        TextAdjustments();
        SaveManager.instance.Save();
    }

    public void RewardAddSuccessCallback(string data)
    {
        NetworkManager.instance.GetUser(GetUserFailCallback, GetUserSuccessCallback);
        var response = JsonUtility.FromJson<AddReward>(data);
        myGold = response.reward;
        NetworkManager.instance.networkMyGolds = myGold;
        canBuy = true;
        TextAdjustments();
        SaveManager.instance.Save();
        if (SaveManager.instance.currentLevel % 3 == 0 && NetworkManager.instance.noAdsBought == false)
            NetworkManager.instance.Invoke("ShowInterstitial", 0);
        NetworkManager.instance.GetUser(GetUserFailCallback, GetUserSuccessCallback);

    }
    public void GetRewardSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<GetReward>(data);
        myGold = response.data.coin;
        NetworkManager.instance.GetUser(NetworkManager.instance.GetUserFailCallback, NetworkManager.instance.GetUserSuccessCallback);
        canBuy = true;
        TextAdjustments();
        SaveManager.instance.Save();
    }


    public void FailCallback(string data)
    {
        var response = JsonUtility.FromJson<FailCallbackError>(data);
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("Not Enough Gold"));
        print(data);
    }

    public void GetUserFailCallback(string data)
    {
        var response = JsonUtility.FromJson<FailCallbackError>(data);
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("Connection Lost!\n Check your connection \n &\n restart the game"));
        print(data);
    }

    private void LoadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);

    }

    void NoAdsBoughtAction()
    {
        if (NetworkManager.instance.noAdsBought == true && noAdsButton != null)
        {
            noAdsButton.SetActive(false);
        }
    }

    public void TextAdjustments()
    {
        //text adjustments
        incomeLevelText.text = SaveManager.instance.incomeLevel.ToString();
        incomeUpgradeCostText.text = SaveManager.instance.incomeUpgradeCost.ToString();

        currentLevelText.text = "LEVEL " + SaveManager.instance.currentLevel.ToString();

        startUnitsLevelText.text = (SaveManager.instance.startUnitsLevel + 1).ToString();
        startUnitsUpgradeCostText.text = SaveManager.instance.startUnitsUpgradeCost.ToString();

        int count = totalBabyList.Count;
        count = Mathf.Clamp(count, 0, 600);

        myGoldsText.text = myGold.ToString();
        walletGoldText.text = myGoldsText.text;
        marketMyGoldsText.text = myGoldsText.text;
        SaveManager.instance.Save();
    }

    private void BabyMoveToLocation()
    {
        // Set a targetPosition variable of where to spawn objects.
        if (playerController != null && playerController.currentPhase == PlayerController.Phases.inGame)
        {
            Vector3 targetPosition = playerController.transform.position - Vector3.forward * 2f;

            // Loop through the number of points in the circle.
            for (int i = 0; i < totalBabyList.Count; i++)
            {
                // Get the angle of the current index being instantiated
                // from the center of the circle.
                float angle = (i) * (3.14159f / 10);
                float myNumber = (.2f + (totalBabyList.Count / 2000));

                // Get the X Position of the angle times 1.5f. 1.5f is the radius of the circle.
                float x = Mathf.Cos(angle) * myNumber;
                // Get the Y Position of the angle times 1.5f. 1.5f is the radius of the circle.
                float y = Mathf.Sin(angle) * myNumber;

                // Set the targetPosition to a new Vector3 with the new variables.
                targetPosition = new Vector3(targetPosition.x + x, targetPosition.y, targetPosition.z + y);

                // Set the position of the instantiated object to the targetPosition.
                //transform.translate uygula
                totalBabyList[i].transform.position = Vector3.Lerp(totalBabyList[i].transform.position, new Vector3(targetPosition.x + x + i / 1000, targetPosition.y, targetPosition.z + y - i / 1000), Time.deltaTime / 6);
            }
        }
    }

    public void UpdateList()
    {
        GameManager.instance.totalBabyList.RemoveAll(x => x == null);
        GameManager.instance.totalBabyList.RemoveAll(x => x.activeSelf == false);
    }

    public void BossIncreaseFightSpeed()
    {
        increaseFightSpeedAnim.Play();
        timeScaleConstant += .05f;
        increaseFightSpeedSlider.value = timeScaleConstant - 1;
        timeScaleConstant = Mathf.Clamp(timeScaleConstant, 0, 2f);
        Time.timeScale = timeScaleConstant;
    }

    public void AttackAdjustment()
    {
        // Attack 
        playerAttackBar.maxValue = playerAttackValue + 12;
        playerAttackBar.value = playerAttackBar.maxValue;
        playerAttackText.text = playerAttackBar.maxValue.ToString();

        //Defense 
        playerDefenseBar.maxValue = playerDefenseValue;
        playerDefenseBar.value = playerDefenseBar.maxValue;
        playerDefenseText.text = playerDefenseBar.maxValue.ToString();

    }

    public void OtherGamesLinkRedirector()
    {
        Application.OpenURL("https://play.google.com/store/apps/dev?id=5698701077378158764&hl=tr&gl=US");
    }

}
