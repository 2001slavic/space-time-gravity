using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject background;

    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject settingsMenu;
    [SerializeField]
    private GameObject singelplayerMenu;
    [SerializeField]
    private GameObject mpLevelSelectMenu;
    [SerializeField]
    private GameObject multiplayerMenu;
    [SerializeField]
    private GameObject networkMPMenu;
    [SerializeField]
    private GameObject splitScreenMenu;
    [SerializeField]
    private GameObject quitMenu;

    [SerializeField]
    private GameObject selectedInMainMenu;
    [SerializeField]
    private GameObject selectedInSettings;
    [SerializeField]
    private GameObject selectedInSP;
    [SerializeField]
    private GameObject selectedLevelMP;
    [SerializeField]
    private GameObject selectedInQuit;
    [SerializeField]
    private GameObject selectedInNetwork;
    [SerializeField]
    private GameObject selectedInNetworkMP;

    [SerializeField]
    private Button joinButton;
    private string joinTo;

    [SerializeField]
    private GameObject navigationButtons;
    [SerializeField]
    private GameObject keyboardNavigation;
    [SerializeField]
    private GameObject xboxNavigation;

    private const int UNDEFINED = 0;
    private const int HOST = 1;
    private const int CLIENT = 2;

    private int networkManagerQuery;

    private float oldVolume;


    // A public method that can be called from other scripts or UI buttons
    private void LoadScene(string sceneName)
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the scene normally
        SceneManager.LoadScene(sceneName);
    }

    // A private method that will be called when a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            joinTo,  // The IP address is a string
            (ushort)22517, // The port number is an unsigned short
            "0.0.0.0" // The server listen address is a string.
        );

        switch (networkManagerQuery)
        {
            case HOST:
                NetworkManager.Singleton.StartHost();
                break;
            case CLIENT:
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
                NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
                oldVolume = PlayerPrefs.GetFloat("volume", 0.5f);
                PlayerPrefs.SetFloat("volume", 0);
                NetworkManager.Singleton.StartClient();
                break;
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        PlayerPrefs.SetFloat("volume", oldVolume);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientid)
    {
        PlayerPrefs.SetFloat("volume", oldVolume);
        Scene oldActiveScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.UnloadSceneAsync(oldActiveScene);
    }

    private void SetCanvasChildrenInactive()
    {
        foreach (Transform child in canvas.gameObject.transform)
        {
            GameObject gameObject = child.gameObject;

            if (gameObject == background || gameObject == navigationButtons)
                continue;
            gameObject.SetActive(false);
        }
    }


    public void SingePlayerClick()
    {
        //mainMenu.SetActive(false);
        //settingsMenu.SetActive(false);
        //quitMenu.SetActive(false);
        ////multiPlayerMenu.SetActive(false);
        //networkMPMenu.SetActive(false);
        SetCanvasChildrenInactive();
        singelplayerMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSP);
    }

    public void SettingsClick()
    {
        //mainMenu.SetActive(false);
        //singelplayerMenu.SetActive(false);
        //quitMenu.SetActive(false);
        ////multiPlayerMenu.SetActive(false);
        //networkMPMenu.SetActive(false);
        SetCanvasChildrenInactive();
        settingsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void QuitMenuClick()
    {
        //mainMenu.SetActive(false);
        //settingsMenu.SetActive(false);
        //singelplayerMenu.SetActive(false);
        ////multiPlayerMenu.SetActive(false);
        //networkMPMenu.SetActive(false);
        SetCanvasChildrenInactive();
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void MPLevelSelectClick()
    {
        SetCanvasChildrenInactive();
        mpLevelSelectMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedLevelMP);
    }

    public void SelectMPTestClick()
    {
        MultiplayerSelectedLevel.level = "testMP";
        MultiplayerMenuClick();
    }

    public void SelectMPLevel1Click()
    {
        MultiplayerSelectedLevel.level = "MP1";
        MultiplayerMenuClick();
    }

    public void MultiplayerMenuClick()
    {
        SetCanvasChildrenInactive();
        multiplayerMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInNetwork);

    }

    public void NetworkMPMenuClick()
    {
        //mainMenu.SetActive(false);
        //settingsMenu.SetActive(false);
        //singelplayerMenu.SetActive(false);
        ////multiPlayerMenu.SetActive(false);
        //quitMenu.SetActive(false);
        SetCanvasChildrenInactive();
        networkMPMenu.SetActive(true);
        
        eventSystem.SetSelectedGameObject(selectedInNetworkMP);
    }

    public void SplitScreenMenuCLick()
    {
        SetCanvasChildrenInactive();
        splitScreenMenu.SetActive(true);

        //eventSystem.SetSelectedGameObject(select);
    }

    public void HostClick()
    {
        networkManagerQuery = HOST;
        LoadScene("TestMP");
    }

    public void ClientClick()
    {
        networkManagerQuery = CLIENT;
        LoadScene("TestMP");
    }

    public void SetIP(string text)
    {

        Regex regex = new Regex("^(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$");

        if (regex.IsMatch(text))
        {
            joinTo = text;
            joinButton.interactable = true;
            Color tmp = joinButton.GetComponentInChildren<TextMeshProUGUI>().color;
            tmp.a = 1;
            joinButton.GetComponentInChildren<TextMeshProUGUI>().color = tmp;
        }
        else
        {
            Color tmp = joinButton.GetComponentInChildren<TextMeshProUGUI>().color;
            tmp.a = 0.5f;
            joinButton.GetComponentInChildren<TextMeshProUGUI>().color = tmp;
            joinButton.interactable = false;
        }
    }

    public void QuitYesClick()
    {
        Application.Quit();
    }

    public void BackToMainMenuClick()
    {
        //singelplayerMenu.SetActive(false);
        //settingsMenu.SetActive(false);
        //quitMenu.SetActive(false);
        ////multiPlayerMenu.SetActive(false);
        //networkMPMenu.SetActive(false);
        SetCanvasChildrenInactive();
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInMainMenu);
    }

    public void TestClick()
    {
        SceneManager.LoadScene("SampleScene");
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void Level1Click()
    {
        SceneManager.LoadScene("SP1");
        SceneManager.UnloadSceneAsync("MainMenu");
    }


    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("sensitivity0", value);
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
    }

    public void SetJoystickSensitivity(float value)
    {
        PlayerPrefs.SetFloat("jsensitivity", value);
    }

    private void HandleLastUsedInputDevice()
    {
        if (splitScreenMenu.activeSelf)
        {
            navigationButtons.SetActive(false);
            return;
        }
        // Check if the last input device is a gamepad
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            navigationButtons.SetActive(true);
            keyboardNavigation.SetActive(false);
            xboxNavigation.SetActive(true);
        }
        // Check if the last input device is a keyboard
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
        {
            navigationButtons.SetActive(true);
            xboxNavigation.SetActive(false);
            keyboardNavigation.SetActive(true);
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        networkManagerQuery = UNDEFINED;
        //singelplayerMenu.SetActive(false);
        //settingsMenu.SetActive(false);
        //quitMenu.SetActive(false);
        ////multiPlayerMenu.SetActive(false);
        //networkMPMenu.SetActive(false);
        SetCanvasChildrenInactive();
        mainMenu.SetActive(true);
        joinTo = "127.0.0.1";
        
    }

    void Update()
    {
        HandleLastUsedInputDevice();
        if (Input.GetButtonDown("Cancel"))
        {
            if (mainMenu.activeSelf)
            {
                QuitMenuClick();
            }
            else if (networkMPMenu.activeSelf || splitScreenMenu.activeSelf)
            {
                MultiplayerMenuClick();
            }
            else if (multiplayerMenu.activeSelf)
            {
                MPLevelSelectClick();
            }
            else
            {
                BackToMainMenuClick();
            }
        }
    }
}
