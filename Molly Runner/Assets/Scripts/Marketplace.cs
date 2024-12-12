using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
public class Marketplace : MonoBehaviour
{
    public static Marketplace instance;
    public ItemScriptable items;

    [SerializeField] private Button hairTitleButton;
    [SerializeField] private Button pantTitleButton;
    [SerializeField] private Button jacketTitleButton;

    #region Marketplace 2D assets
    [SerializeField] private Image hatImage;
    [SerializeField] private Image jacketImage;
    [SerializeField] private Image pantImage;
    [SerializeField] private MeshRenderer hatMeshR;
    [SerializeField] private MeshFilter hatMeshF;
    [SerializeField] private SkinnedMeshRenderer jacketRenderer;
    [SerializeField] private SkinnedMeshRenderer pantRenderer;
    #endregion

    #region Meshes & Materials
    [SerializeField] private List<Mesh> hatMeshes;
    [SerializeField] private List<Mesh> jacketMeshes;
    [SerializeField] private List<Mesh> pantMeshes;

    [SerializeField] private List<Material> hatMaterials;
    [SerializeField] private List<Material> jacketMaterials;
    [SerializeField] private List<Material> pantMaterials;

    #endregion

    public List<GameObject> hatLockImages;
    public List<GameObject> jacketLockImages;
    public List<GameObject> pantLockImages;

    public Transform buyTab;
    [SerializeField] private Image buyItemImage;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI buyTabPriceText;
    private int item_id;


    private int attempt;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //addlistener to clothes tabs
        hairTitleButton.onClick.AddListener(HairTab);
        pantTitleButton.onClick.AddListener(PantTab);
        jacketTitleButton.onClick.AddListener(JacketTab);
        purchaseButton.onClick.AddListener(PurchaseButton);
        closeButton.onClick.AddListener(CloseButtonOperation);

        //activate wore clothes
        UIManager.instance.GameStartScreen();
        UIManager.instance.marketplacePanel.SetActive(true);

        NetworkManager.instance.ItemGetForUser(GetUserFailCallback, ItemGetForUserSuccessCallback);


        UIManager.instance.marketplacePanel.SetActive(false);

    }


    void HairTab()
    {
        hairTitleButton.transform.GetChild(0).gameObject.SetActive(true);
        pantTitleButton.transform.GetChild(0).gameObject.SetActive(false);
        jacketTitleButton.transform.GetChild(0).gameObject.SetActive(false);
        SaveManager.instance.Save();
    }

    void PantTab()
    {
        hairTitleButton.transform.GetChild(0).gameObject.SetActive(false);
        pantTitleButton.transform.GetChild(0).gameObject.SetActive(true);
        jacketTitleButton.transform.GetChild(0).gameObject.SetActive(false);
        SaveManager.instance.Save();
    }

    void JacketTab()
    {
        hairTitleButton.transform.GetChild(0).gameObject.SetActive(false);
        pantTitleButton.transform.GetChild(0).gameObject.SetActive(false);
        jacketTitleButton.transform.GetChild(0).gameObject.SetActive(true);
        SaveManager.instance.Save();
    }

    public void SelectHat(int index)
    {

        if (hatLockImages[index].activeSelf == false)
        {
            hatImage.sprite = items.hats[index].woreImage;
            hatMeshR.material = hatMaterials[index];
            hatMeshF.sharedMesh = hatMeshes[index];
            SaveManager.instance.currentHat = index;

        }
        if (hatLockImages[index].activeSelf == true)
        {
            buyTab.gameObject.SetActive(true);
            buyItemImage.sprite = items.hats[index].cutImage;
            item_id = items.hats[index].itemID;

        }
        NetworkManager.instance.ItemGetForUser(GetUserFailCallback, ItemGetForUserSuccessCallback);

    }

    public void SelectJacket(int index)
    {

        if (jacketLockImages[index].activeSelf == false)
        {
            jacketImage.sprite = items.jackets[index].woreImage;
            jacketRenderer.material = jacketMaterials[index];
            jacketRenderer.sharedMesh = jacketMeshes[index];
            SaveManager.instance.currentJacket = index;
        }
        if (jacketLockImages[index].activeSelf == true)
        {
            buyTab.gameObject.SetActive(true);
            buyItemImage.sprite = items.jackets[index].cutImage;
            item_id = items.jackets[index].itemID;
        }
        NetworkManager.instance.ItemGetForUser(GetUserFailCallback, ItemGetForUserSuccessCallback);

    }
    public void SelectPant(int index)
    {

        if (pantLockImages[index].activeSelf == false)
        {
            pantImage.sprite = items.pants[index].woreImage;
            pantRenderer.material = pantMaterials[index];
            pantRenderer.sharedMesh = pantMeshes[index];
            SaveManager.instance.currentPant = index;
        }
        if (pantLockImages[index].activeSelf == true)
        {
            buyTab.gameObject.SetActive(true);
            buyItemImage.sprite = items.pants[index].cutImage;
            item_id = items.pants[index].itemID;
        }
        NetworkManager.instance.ItemGetForUser(GetUserFailCallback, ItemGetForUserSuccessCallback);

    }
    void WearClothes()
    {

        if (pantLockImages[SaveManager.instance.currentPant].activeSelf == false)
        {
            pantRenderer.material = pantMaterials[SaveManager.instance.currentPant];
            pantRenderer.sharedMesh = pantMeshes[SaveManager.instance.currentPant];
        }

        if (jacketLockImages[SaveManager.instance.currentJacket].activeSelf == false)
        {
            jacketRenderer.material = jacketMaterials[SaveManager.instance.currentJacket];
            jacketRenderer.sharedMesh = jacketMeshes[SaveManager.instance.currentJacket];
        }

        if (hatLockImages[SaveManager.instance.currentHat].activeSelf == false)
        {
            hatMeshR.material = hatMaterials[SaveManager.instance.currentHat];
            hatMeshF.sharedMesh = hatMeshes[SaveManager.instance.currentHat];
        }
    }

    void PurchaseButton()
    {
        NetworkManager.instance.ItemAddForUser(AddForUserFailCallback, ItemAddForUserSuccessCallback, item_id);
    }

    void CloseButtonOperation()
    {
        StartCoroutine(CloseButton());
    }

    public IEnumerator CloseButton()
    {
        buyTab.GetComponent<Animation>().Play("ReverseBuyTab");
        yield return new WaitForSeconds(buyTab.GetComponent<Animation>().GetClip("ReverseBuyTab").length);
        buyTab.gameObject.SetActive(false);
        SaveManager.instance.Save();
    }

    void ItemAddForUserSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<ItemAddForUser>(data);
        NetworkManager.instance.networkMyGolds = response.coin;

        StartCoroutine(CloseButton());

        NetworkManager.instance.ItemGetForUser(GetUserFailCallback, ItemGetForUserSuccessCallback);
        if (GameManager.instance.gameObject != null)
            NetworkManager.instance.RewardGet(GameManager.instance.FailCallback, GameManager.instance.GetRewardSuccessCallback);

        SaveManager.instance.Save();

    }

    public void ItemGetForUserSuccessCallback(string data)
    {
        var response = JsonUtility.FromJson<Item>(data);
        if (data.Length > 0)
        {
            // LockImages
            for (int i = 0; i < 9; i++)
            {
                if (response.data[i].isLocked == true && i != 0)
                {
                    hatLockImages[i].gameObject.SetActive(true);
                }
                else
                {
                    hatLockImages[i].gameObject.SetActive(false);
                }
            }
            for (int i = 9; i < 14; i++)
            {
                if (response.data[i].isLocked == true)
                {
                    jacketLockImages[i - 9].gameObject.SetActive(true);
                }
                else
                {
                    jacketLockImages[i - 9].gameObject.SetActive(false);
                }
            }
            for (int i = 14; i < 19; i++)
            {
                if (response.data[i].isLocked == true)
                {
                    pantLockImages[i - 14].gameObject.SetActive(true);
                }
                else
                {
                    pantLockImages[i - 14].gameObject.SetActive(false);
                }

            }

            if (item_id != 0)
            {
                buyTabPriceText.text = response.data[item_id - 1].in_game_price.ToString();
                buyTabAttack.text = response.data[item_id - 1].attack_value.ToString();
                buyTabDefense.text = response.data[item_id - 1].defence_value.ToString();
            }


            if (GameManager.instance.gameObject != null)
            {
                GameManager.instance.playerAttackValue = items.hats[SaveManager.instance.currentHat].attack + items.jackets[SaveManager.instance.currentJacket].attack + items.pants[SaveManager.instance.currentPant].attack;
                GameManager.instance.playerDefenseValue = items.hats[SaveManager.instance.currentHat].defenceValue + items.jackets[SaveManager.instance.currentJacket].defenceValue + items.pants[SaveManager.instance.currentPant].defenceValue;
                GameManager.instance.TextAdjustments();
            }
            SaveManager.instance.Save();
            WearClothes();
        }

    }
    [SerializeField] private TextMeshProUGUI buyTabAttack;
    [SerializeField] private TextMeshProUGUI buyTabDefense;

    public void AddForUserFailCallback(string data)
    {
        var response = JsonUtility.FromJson<FailCallbackError>(data);
        NetworkManager.instance.isNetworkSuccess = response.success;
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("Not enough gold"));
    }

    public void GetUserFailCallback(string data)
    {
        var response = JsonUtility.FromJson<FailCallbackError>(data);
        NetworkManager.instance.isNetworkSuccess = response.success;
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("Network Error"));
    }
}