using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;
    public Slider progressBar;
    public GameObject splashScreen;
    public GameObject signScreen;
    [SerializeField] private TextMeshProUGUI loadPercentageText;
    private float loadPercentage;
    private bool levelLoading = false;

    void Awake()
    {
        instance = this;
    }

    public void LoadLevel(string level)
    {
        progressBar.gameObject.SetActive(true);
        StartCoroutine(StartLoading(level));

    }

    IEnumerator StartLoading(string level)
    {
        if (levelLoading == false)
        {
            splashScreen.SetActive(true);

            AsyncOperation async = SceneManager.LoadSceneAsync(level);

            while (!async.isDone)
            {
                loadPercentage = progressBar.value * 100;

                loadPercentageText.text = ((int)loadPercentage).ToString() + "%";
                progressBar.value = async.progress;
                yield return null;
            }
        }

    }

}
