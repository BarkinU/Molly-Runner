using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance { get; private set; }

    #region Hats
    //What we want to save
    public int currentHat;
    #endregion

    #region jackets
    //What we want to save
    public int currentJacket;
    #endregion

    #region Pants
    //What we want to save
    public int currentPant;
    #endregion

    public int currentLevel = 1;
    public float musicVolume;
    public float sfxVolume;
    public int startUnitsLevel = 1;
    public int incomeLevel = 1;
    public int incomeUpgradeCost = 250;
    public int startUnitsUpgradeCost = 50;
    public bool noAdsOwned;
    public int deathProtectionAmount;
    public int powerUpAgainstBossAmount;
    public int instantX2Amount;
    public int increaseSpeedAmount;
    public int reduceMollySizeAmount;
    public int jumpAmount;

    private void Awake()
    {

        instance = this;

        DontDestroyOnLoad(gameObject);
        Loader();
    }

    void Loader()
    {
        Load();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData_Storage data = (PlayerData_Storage)bf.Deserialize(file);

            currentHat = data.currentHat;
            currentJacket = data.currentJacket;
            currentPant = data.currentPant;

            incomeUpgradeCost = data.incomeUpgradeCost;
            startUnitsUpgradeCost = data.startUnitsUpgradeCost;

            currentLevel = data.currentLevel;
            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;

            //market data
            noAdsOwned = data.noAdsOwned;
            jumpAmount = data.jumpAmount;
            instantX2Amount = data.instantX2Amount;
            increaseSpeedAmount = data.increaseSpeedAmount;
            reduceMollySizeAmount = data.reduceMollySizeAmount;
            deathProtectionAmount = data.deathProtectionAmount;
            powerUpAgainstBossAmount = data.powerUpAgainstBossAmount;

            file.Close();

        }



    }
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        PlayerData_Storage data = new PlayerData_Storage();

        data.currentHat = currentHat;

        data.currentJacket = currentJacket;

        data.currentPant = currentPant;

        data.incomeUpgradeCost = incomeUpgradeCost;
        data.startUnitsUpgradeCost = startUnitsUpgradeCost;

        data.currentLevel = currentLevel;
        data.musicVolume = musicVolume;
        data.sfxVolume = sfxVolume;

        //market data
        data.noAdsOwned = noAdsOwned;
        data.jumpAmount = jumpAmount;
        data.instantX2Amount = instantX2Amount;
        data.increaseSpeedAmount = increaseSpeedAmount;
        data.reduceMollySizeAmount = reduceMollySizeAmount;
        data.deathProtectionAmount = deathProtectionAmount;
        data.powerUpAgainstBossAmount = powerUpAgainstBossAmount;

        bf.Serialize(file, data);
        file.Close();
    }
}

[Serializable]
class PlayerData_Storage
{
    public int currentHat;
    public int currentJacket;
    public int currentPant;

    public int currentLevel = 1;
    public float musicVolume;
    public float sfxVolume;
    public int startUnitsLevel = 1;
    public int incomeLevel = 1;
    public int incomeUpgradeCost = 250;
    public int startUnitsUpgradeCost = 50;
    public bool noAdsOwned;
    public int deathProtectionAmount;
    public int powerUpAgainstBossAmount;
    public int instantX2Amount;
    public int increaseSpeedAmount;
    public int reduceMollySizeAmount;
    public int jumpAmount;


}