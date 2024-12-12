using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager instance;
    public TMP_InputField loginUsernameIF;
    public TMP_InputField loginPasswordIF;
    public TMP_InputField registerUsernameIF;
    public TMP_InputField registerPasswordIF;
    public TMP_InputField registerPasswordConfirmIF;
    public Button loginButton;
    public Button registerButton;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject welcomeBackPanel;
    public TMP_Text welcomeBackText;


    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
        {
            loginUsernameIF.text = PlayerPrefs.GetString("username");
            loginPasswordIF.text = PlayerPrefs.GetString("password");
            welcomeBackText.text = "Welcome back " + loginUsernameIF.text;
            welcomeBackPanel.SetActive(true);
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            Login();
        }
        else
        {
            registerPanel.SetActive(true);
            loginPanel.SetActive(false);
            welcomeBackPanel.SetActive(false);
        }
    }

    private void Register()
    {
        NetworkManager.instance.AuthRegister(NetworkManager.instance.FailCallback, NetworkManager.instance.RegisterSuccessCallback);
    }

    private void Login()
    {
        NetworkManager.instance.AuthLogin(NetworkManager.instance.FailCallback, NetworkManager.instance.LoginSuccessCallback, loginUsernameIF.text, loginPasswordIF.text);
    }
}
