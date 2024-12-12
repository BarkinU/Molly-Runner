using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class VFXMarketplace : MonoBehaviour
{
    public static VFXMarketplace instance;
    public int deathProtectionAmount;
    public int powerUpAmount;
    public int instantX2MollyAmount;
    public int increaseSpeedAmount;
    public int reduceMollySizeAmount;
    public int jumpAmount;

    public string deathProtectionName = "com.minego.runner.damageprotection";
    public string powerUpName = "com.minego.runner.powerup";
    public string instantX2Name = "com.minego.runner.instantx2";
    public string increaseSpeedName = "com.minego.runner.increasespeed";
    public string reduceMollySizeName = "com.minego.runner.reducemollysize";
    public string jumpName = "com.minego.runner.jump";
    public string noAdsName = "com.minego.runner.noads";
    public string ninetyPercentageBundlePack = "com.minego.runner.ninetypercentbundlepack";

    [SerializeField] private TextMeshProUGUI haveAmount;

    public GameObject _deathProtectionAm;
    public GameObject _powerUpAm;
    public GameObject _instantX2MollyAm;
    public GameObject _increaseSpeedAm;
    public GameObject _reduceMollySizeAm;
    public GameObject _jumpAmo;
    [SerializeField] private GameObject loadingCanvas;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ClearScreen();
        _deathProtectionAm.SetActive(true);

        //setting up amounts;
        GetHaveAmount();
        haveAmount.text = deathProtectionAmount.ToString();
    }

    void GetHaveAmount()
    {
        jumpAmount = SaveManager.instance.jumpAmount;
        powerUpAmount = SaveManager.instance.powerUpAgainstBossAmount;
        increaseSpeedAmount = SaveManager.instance.increaseSpeedAmount;
        reduceMollySizeAmount = SaveManager.instance.reduceMollySizeAmount;
        instantX2MollyAmount = SaveManager.instance.instantX2Amount;
        deathProtectionAmount = SaveManager.instance.deathProtectionAmount;
    }

    void SaveHaveAmount()
    {
        SaveManager.instance.jumpAmount = jumpAmount;
        SaveManager.instance.powerUpAgainstBossAmount = powerUpAmount;
        SaveManager.instance.increaseSpeedAmount = increaseSpeedAmount;
        SaveManager.instance.reduceMollySizeAmount = reduceMollySizeAmount;
        SaveManager.instance.deathProtectionAmount = deathProtectionAmount;
        SaveManager.instance.deathProtectionAmount = deathProtectionAmount;
    }
    void ClearScreen()
    {
        _deathProtectionAm.SetActive(false);
        _powerUpAm.SetActive(false);
        _instantX2MollyAm.SetActive(false);
        _increaseSpeedAm.SetActive(false);
        _reduceMollySizeAm.SetActive(false);
        _jumpAmo.SetActive(false);
    }
    public void LoadingOpen()
    {
        loadingCanvas.SetActive(true);
    }
    public void Restore()
    {
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("You may need to restart the game after restoring"));
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == deathProtectionName)
        {
            deathProtectionAmount++;
            SaveManager.instance.deathProtectionAmount++;
            haveAmount.text = deathProtectionAmount.ToString();
            VFXManager.instance.deathProtectionButton.interactable = true;
        }
        if (product.definition.id == powerUpName)
        {
            powerUpAmount++;
            SaveManager.instance.powerUpAgainstBossAmount++;
            haveAmount.text = powerUpAmount.ToString();
            VFXManager.instance.powerUpAgainstButton.interactable = true;

        }
        if (product.definition.id == instantX2Name)
        {
            SaveManager.instance.instantX2Amount++;
            instantX2MollyAmount++;
            haveAmount.text = instantX2MollyAmount.ToString();
            VFXManager.instance.instanX2Button.interactable = true;
        }
        if (product.definition.id == increaseSpeedName)
        {
            SaveManager.instance.increaseSpeedAmount++;
            increaseSpeedAmount++;
            haveAmount.text = increaseSpeedAmount.ToString();
            VFXManager.instance.increaseSpeedButton.interactable = true;
        }
        if (product.definition.id == reduceMollySizeName)
        {
            SaveManager.instance.reduceMollySizeAmount++;
            reduceMollySizeAmount++;
            haveAmount.text = reduceMollySizeAmount.ToString();
            VFXManager.instance.reduceMollySizeButton.interactable = true;
        }
        if (product.definition.id == jumpName)
        {
            SaveManager.instance.jumpAmount++;
            jumpAmount++;
            haveAmount.text = jumpAmount.ToString();
            VFXManager.instance.jumpButton.interactable = true;
        }
        if (product.definition.id == noAdsName)
        {
            NetworkManager.instance.noAdsBought = true;
            SaveManager.instance.noAdsOwned = true;
            SaveManager.instance.Save();
            NetworkManager.instance.HideBanner();
            SaveHaveAmount();
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        if (product.definition.id == ninetyPercentageBundlePack)
        {

            deathProtectionAmount += 2;
            jumpAmount += 5;
            powerUpAmount += 2;
            instantX2MollyAmount += 3;
            increaseSpeedAmount += 3;
            reduceMollySizeAmount += 5;

            SaveManager.instance.deathProtectionAmount += 2;
            SaveManager.instance.jumpAmount += 5;
            SaveManager.instance.powerUpAgainstBossAmount += 2;
            SaveManager.instance.instantX2Amount += 3;
            SaveManager.instance.increaseSpeedAmount += 3;
            SaveManager.instance.reduceMollySizeAmount += 5;

            haveAmount.text = deathProtectionAmount.ToString();

            VFXManager.instance.deathProtectionButton.interactable = true;
            VFXManager.instance.jumpButton.interactable = true;
            VFXManager.instance.powerUpAgainstButton.interactable = true;
            VFXManager.instance.instanX2Button.interactable = true;
            VFXManager.instance.increaseSpeedButton.interactable = true;
            VFXManager.instance.reduceMollySizeButton.interactable = true;
        }

        loadingCanvas.SetActive(false);
        SaveHaveAmount();
        SaveManager.instance.Save();
        VFXManager.instance.TextAdjustment();

    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        loadingCanvas.SetActive(false);
        NetworkManager.instance.StartCoroutine(NetworkManager.instance.ErrorPanel("Purchase Unsuccesful"));
    }

    public void SelectDeathProtection()
    {
        ClearScreen();
        haveAmount.text = deathProtectionAmount.ToString();
        _deathProtectionAm.SetActive(true);
    }

    public void SelectPowerUp()
    {
        ClearScreen();
        haveAmount.text = powerUpAmount.ToString();
        _powerUpAm.SetActive(true);

    }

    public void SelectInstantX2Molly()
    {
        ClearScreen();
        haveAmount.text = instantX2MollyAmount.ToString();
        _instantX2MollyAm.SetActive(true);
    }

    public void SelectIncreaseSpeed()
    {
        ClearScreen();
        haveAmount.text = increaseSpeedAmount.ToString();
        _increaseSpeedAm.SetActive(true);
    }

    public void SelectReduceSize()
    {
        ClearScreen();
        haveAmount.text = reduceMollySizeAmount.ToString();
        _reduceMollySizeAm.SetActive(true);
    }

    public void SelectJump()
    {
        ClearScreen();
        haveAmount.text = jumpAmount.ToString();
        _jumpAmo.SetActive(true);

    }

    public void NinetyPercentageDiscount()
    {



    }

}
