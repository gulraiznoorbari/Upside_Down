using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LootLocker.Requests;


public class WhiteLabelLoginManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject registerScreen;

    // Input fields
    [Header("New User")]
    public TMP_InputField newUserNameInputField;
    public TMP_InputField newUserEmailInputField;
    public TMP_InputField newUserPasswordInputField;

    [Header("Existing User")]
    public TMP_InputField existingUserEmailInputField;
    public TMP_InputField existingUserPasswordInputField;

    [Header("Player")]
    public TextMeshProUGUI playerNameText;
    public int playerID;
    public TextMeshProUGUI playerIDText;

    [Header("RememberMe")]
    // Components for enabling auto login
    public Toggle rememberMeToggle;
    private int rememberMe;

    [Header("Error Message")]
    public TextMeshProUGUI errorNameText;


    private static WhiteLabelLoginManager instance;
    // Singleton Instantiation:
    public static WhiteLabelLoginManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<WhiteLabelLoginManager>();
            return instance;
        }
    }


    // Start is called before the first frame update
    public void Start()
    {
        errorNameText.text = "";
        registerScreen.SetActive(false);
        loginScreen.SetActive(false);
        startScreen.SetActive(true);

        // See if we should log in the player automatically
        rememberMe = PlayerPrefs.GetInt("rememberMe", 0);
        if (rememberMe == 0)
        {
            rememberMeToggle.isOn = false;
        }
        else
        {
            rememberMeToggle.isOn = true;
        }
    }

    public void Register()
    {
        registerScreen.SetActive(true);
        startScreen.SetActive(false);

        string email = newUserEmailInputField.text;
        string password = newUserPasswordInputField.text;
        string username = newUserNameInputField.text;

        if (email == "" || password == "")
        {
            Debug.Log("Field is Empty");
            errorNameText.text = "Please fill both the fields";
            return;
        }
        LootLockerSDKManager.WhiteLabelSignUp(email, password, (registerResponse) =>
        {
            if (!registerResponse.success)
            {
                errorNameText.text = "Error while user signup";
                Debug.Log("error while creating user");
                return;
            }
            else
            {
                LootLockerSDKManager.WhiteLabelLogin(email, password, (loginResponse) =>
                {
                    if (!loginResponse.success)
                    {
                        Debug.Log("Login Error");
                        errorNameText.text = "Error while logging in";
                        return;
                    }
                    else {
                        errorNameText.text = "";
                        LootLockerSDKManager.StartWhiteLabelSession((startSessionResponse) =>
                        {
                            if (!startSessionResponse.success)
                            {
                                Debug.Log("Login Error");
                                errorNameText.text = "Error while logging in";
                                return;
                            }
                            else
                            {
                                errorNameText.text = "";
                                if (username == "")
                                {
                                    username = startSessionResponse.public_uid;
                                }
                                LootLockerSDKManager.SetPlayerName(username, (response) =>
                                {
                                    if (!response.success)
                                    {
                                        Debug.Log(response.Error);
                                        errorNameText.text = response.Error;
                                        return;
                                    }
                                    errorNameText.text = "";
                                    PlayerPrefs.SetString("pname", response.name);

                                    // end the session:
                                    LootLockerSessionRequest sessionRequest = new LootLockerSessionRequest();
                                    LootLocker.LootLockerAPIManager.EndSession(sessionRequest, (endSessionResponse) =>
                                    {
                                        if (!endSessionResponse.success)
                                        {
                                            Debug.Log(endSessionResponse.Error);
                                            errorNameText.text = endSessionResponse.Error;
                                            return;
                                        }
                                        Debug.Log("Account Created");
                                        errorNameText.text = "";
                                        PlayerPrefs.SetInt("pid", endSessionResponse.player_id);
                                        registerScreen.SetActive(false);
                                        AutoLogin();
                                        rememberMeToggle.isOn = false;
                                    });
                                });
                            }
                        });
                    }
                });
            }
        });
    }


    public void Login()
    {
        loginScreen.SetActive(true);
        startScreen.SetActive(false);
        string email = existingUserEmailInputField.text;
        string password = existingUserPasswordInputField.text;
        if (email == "" || password == "")
        {
            Debug.Log("Field is Empty");
            errorNameText.text = "Please fill both the fields";
            return;
        }
        try {
            LootLockerSDKManager.WhiteLabelLogin(email, password, Convert.ToBoolean(rememberMe), loginResponse =>
            {
                if (!loginResponse.success)
                {
                    Debug.Log("Login Error");
                    errorNameText.text = "Error while logging in";
                    return;
                }
                else
                {
                    Debug.Log("Player was logged in succesfully");
                    errorNameText.text = "";
                    Debug.Log(loginResponse);
                }

                if (loginResponse.VerifiedAt == null)
                {
                    // Stop here if you want to require your players to verify their email before continuing,
                    // verification must also be enabled on the LootLocker dashboard
                }

                // Player is logged in, now start a game session
                LootLockerSDKManager.StartWhiteLabelSession((startSessionResponse) =>
                {
                    if (startSessionResponse.success)
                    {
                        errorNameText.text = "";
                        playerID = startSessionResponse.player_id;
                        Debug.Log("Player ID: " + playerID);
                        Debug.Log("Session started");
                        SetPlayerNameOnScreen();
                    }
                    else
                    {
                        // Error
                        errorNameText.text = "Error while starting LootLocker session.";
                        Debug.Log("Error while starting session");
                        return;
                    }
                });
            });
        }
        catch (Exception exp) {
            Debug.Log(exp.Message);
            Debug.Log("Error found");
            Start();
            return;
        }
    }


    public void AutoLogin()
    {
        // Does the user want to automatically log in?
        if (Convert.ToBoolean(rememberMe) == true)
        {            
            LootLockerSDKManager.CheckWhiteLabelSession(response =>
            {
                if (response == false)
                {
                    // Session was not valid, show error animation and show back button

                    // set the remember me bool to false here, so that the next time the player press login
                    // they will get to the login screen
                    rememberMeToggle.isOn = false;
                }
                else
                {
                    // Session is valid, start game session
                    LootLockerSDKManager.StartWhiteLabelSession((sessionResponse) =>
                    {
                        if (sessionResponse.success)
                        {
                            PlayerPrefs.SetInt("pid", sessionResponse.player_id);
                            SetPlayerNameOnScreen();
                        }
                        else
                        {
                            Debug.Log("error starting LootLocker session");
                            rememberMeToggle.isOn = false;
                            return;
                        }
                    });
                }
            });
        }
        else if (Convert.ToBoolean(rememberMe) == false)
        {
            loginScreen.SetActive(true);
            startScreen.SetActive(false);

        }
    }


    private void SetPlayerNameOnScreen()
    {
        loginScreen.SetActive(false);
        registerScreen.SetActive(false);
        startScreen.SetActive(false);

        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                errorNameText.text = "";

                PlayerPrefs.SetString("pname", response.name);
                string name = PlayerPrefs.GetString("pname");
                int pid = PlayerPrefs.GetInt("pid");

                playerNameText.text = name;
                playerIDText.text = pid.ToString();

                Debug.Log("Player Name: " + response.name + " ID: " + pid);
            }
            else
            {
                errorNameText.text = "Error getting player name";
                return;
            }
        });
    }


    // Called when changing the value on the toggle
    public void ToggleRememberMe()
    {
        bool rememberMeBool = rememberMeToggle.isOn;
        rememberMe = Convert.ToInt32(rememberMeBool);
        PlayerPrefs.SetInt("rememberMe", rememberMe);
    }
}
