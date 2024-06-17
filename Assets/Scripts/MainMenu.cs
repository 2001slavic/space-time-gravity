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
    private GameObject controlsMenu;
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
    [SerializeField]
    private GameObject playstationNavigation;

    private const int UNDEFINED = 0;
    private const int HOST = 1;
    private const int CLIENT = 2;

    private int networkManagerQuery;

    private float oldVolume;

    [SerializeField]
    private TMP_InputField sensitivityInputField;
    [SerializeField]
    private Slider sensitivitySlider;

    private string lastUsedInputDevice;


    // subscribe to scene loaded event and start loading multiplayer scene
    private void LoadScene(string sceneName)
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the scene normally
        SceneManager.LoadScene(sceneName);
    }

    // set role: whether host or client
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            joinTo,  // ip address to join to (used as client)
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
        PerformanceLogger.gamePaused = true;
        PlayerPrefs.SetFloat("volume", oldVolume);
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
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
        SetCanvasChildrenInactive();
        singelplayerMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSP);
    }

    public void SettingsClick()
    {
        SetCanvasChildrenInactive();
        settingsMenu.SetActive(true);
        sensitivityInputField.text = SensitivityFloatToString(GetMouseSensitivity());
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void ControlsClick()
    {
        SetCanvasChildrenInactive();
        controlsMenu.SetActive(true);
    }

    public void QuitMenuClick()
    {
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
        SetCanvasChildrenInactive();
        networkMPMenu.SetActive(true);
        
        eventSystem.SetSelectedGameObject(selectedInNetworkMP);
    }

    public void SplitScreenMenuCLick()
    {
        SetCanvasChildrenInactive();
        splitScreenMenu.SetActive(true);
    }

    public void HostClick()
    {
        networkManagerQuery = HOST;
        LoadScene(MultiplayerSelectedLevel.level + "Network");
    }

    public void ClientClick()
    {
        networkManagerQuery = CLIENT;
        LoadScene(MultiplayerSelectedLevel.level + "Network");
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
        SetCanvasChildrenInactive();
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInMainMenu);
    }

    public void TestClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Level1Click()
    {
        SceneManager.LoadScene("SP1");
    }

    public void Level2Click()
    {
        SceneManager.LoadScene("SP2");
    }

    private string SensitivityFloatToString(float value)
    {
        string res = Mathf.FloorToInt(value).ToString();
        return res.Substring(0, Mathf.Min(res.Length, 4));
    }

    public float GetMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("sensitivity0", 500);
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("sensitivity0", value);
        sensitivityInputField.text = SensitivityFloatToString(value);
    }
    public void SetMouseSensitivity(string value)
    {
        float floatValue = float.Parse(value);
        PlayerPrefs.SetFloat("sensitivity0", floatValue);
        sensitivitySlider.value = floatValue;
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
    }

    public void SetJoystickSensitivity(float value)
    {
        PlayerPrefs.SetFloat("jsensitivity", value);
    }

    private void GetLastUsedInputDevice()
    {
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
        {
            if (Gamepad.current.name.Contains("PlayStation"))
            {
                lastUsedInputDevice = "PlayStation";
            }
            else
            {
                lastUsedInputDevice = "Xbox";
            }

        }
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
        {
            lastUsedInputDevice = "Keyboard";
        }
    }

    private void ChangeNavigationButtons()
    {
        if (splitScreenMenu.activeSelf)
        {
            navigationButtons.SetActive(false);
            return;
        }

        GetLastUsedInputDevice();

        if (lastUsedInputDevice == "PlayStation")
        {
            navigationButtons.SetActive(true);
            xboxNavigation.SetActive(false);
            keyboardNavigation.SetActive(false);
            playstationNavigation.SetActive(true);
        }
        else if (lastUsedInputDevice == "Xbox")
        {
            navigationButtons.SetActive(true);
            keyboardNavigation.SetActive(false);
            playstationNavigation.SetActive(false);
            xboxNavigation.SetActive(true);
        }
        else
        {
            navigationButtons.SetActive(true);
            xboxNavigation.SetActive(false);
            playstationNavigation.SetActive(false);
            keyboardNavigation.SetActive(true);
        }
    }

    private void ChangeControlsLayout()
    {
        if (!controlsMenu.activeSelf)
        {
            return;
        }
        GetLastUsedInputDevice();

        foreach (Transform child in controlsMenu.transform)
        {
            Button backButton = child.GetComponent<Button>();
            if (backButton != null)
            {
                eventSystem.SetSelectedGameObject(backButton.gameObject);
                continue;
            }
            child.gameObject.SetActive(false);
        }

        if (lastUsedInputDevice == "PlayStation")
        {
            GetChildByName.Get(controlsMenu, "PlayStationControls").SetActive(true);
        }
        else if (lastUsedInputDevice == "Xbox")
        {
            GetChildByName.Get(controlsMenu, "XboxControls").SetActive(true);
        }
        else
        {
            GetChildByName.Get(controlsMenu, "KeyboardControls").SetActive(true);
        }
    }

    private void Awake()
    {
        PerformanceLogger.gamePaused = true;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        networkManagerQuery = UNDEFINED;
        SetCanvasChildrenInactive();
        mainMenu.SetActive(true);
        joinTo = "127.0.0.1";
        lastUsedInputDevice = "Keyboard";
    }

    void Update()
    {
        ChangeNavigationButtons();
        ChangeControlsLayout();
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
