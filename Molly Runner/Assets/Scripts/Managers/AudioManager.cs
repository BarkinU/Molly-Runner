using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] source;
    private int currentLevel;
    private int levelCount;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private AudioMixer musicMixer;
    
    public AudioClip[] clips;

    void Awake()
    {
        // Instance filling
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        levelCount = SceneManager.sceneCountInBuildSettings;
        currentLevel = PlayerPrefs.GetInt("currentLevel", currentLevel);
        currentLevel = Mathf.Clamp(currentLevel, 1, levelCount);
        musicMixer.SetFloat("volume", PlayerPrefs.GetFloat("MusicVolume"));
        sfxMixer.SetFloat("volume", PlayerPrefs.GetFloat("SFXVolume"));
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

    }

    public void Play(string name)
    {
        source[0].clip = Array.Find(clips, x => x.name == name);
        if (source[0].isPlaying == false)
            source[0].Play();
    }
    public void Stop(string name)
    {
        source[0].Stop();
    }


    public void SetMusicVolume(float volume)
    {
        musicMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);

    }

    public void SetSFXVolume(float volume)
    {
        sfxMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);

    }

    public void ButtonBack()
    {

        Play("MenuBack");

    }

    public void ButtonClick()
    {

        Play("MenuClick");

    }

}
