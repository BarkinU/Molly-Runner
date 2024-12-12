using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class SplashManager1 : MonoBehaviour
{
    public static SplashManager1 instance;

    void Awake()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        if (instance != this && instance == null)
        {
            instance = this;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void CheckNetwork()
    {
        LoadManager.instance.LoadLevel("Level");
    }
}