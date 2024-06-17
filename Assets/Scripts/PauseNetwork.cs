using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseNetwork : NetworkBehaviour
{
    public bool gamePaused;
    public Canvas pauseCanvas;
    public Camera pauseCamera;
    public Camera playerCamera;

    public EventSystem eventSystem;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    [SerializeField]
    private GameObject controlsMenu;
    public GameObject quitMenu;
    public GameObject hostWaitMenu;

    public GameObject selectedInMain;
    public GameObject selectedInSettings;
    public GameObject selectedInQuit;
    public GameObject selectedInHostWait;

    [SerializeField]
    private PlayerLookNetwork playerLook;

    public bool waitForSecondPlayer;

    private SpawnPositions spawnPositions;

    private string lastUsedInputDevice;

    [SerializeField]
    private TMP_InputField sensitivityInputField;
    [SerializeField]
    private Slider sensitivitySlider;

    public void ShowHostWaitScreen()
    {
        if (!IsOwner) return;
        playerCamera.gameObject.SetActive(false);
        pauseCanvas.gameObject.SetActive(true);
        pauseCamera.gameObject.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        mainMenu.SetActive(false);
        controlsMenu.SetActive(false);
        hostWaitMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        eventSystem.SetSelectedGameObject(selectedInHostWait);
    }

    public void ResumeClick()
    {
        if (!IsOwner) return;
        gamePaused = false;
        PerformanceLogger.gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        controlsMenu.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        playerLook.x = 0;
        playerLook.y = 0;
    }

    public void SettingsClick()
    {
        if (!IsOwner) return;
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        controlsMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        settingsMenu.SetActive(true);
        sensitivityInputField.text = SensitivityFloatToString(GetMouseSensitivity());
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }
    public void ControlsClick()
    {
        if (!IsOwner) return;
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void QuitMenuClick()
    {
        if (!IsOwner) return;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        controlsMenu.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void BackToPauseMenuClick()
    {
        if (!IsOwner) return;
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInMain);
    }

    public void ToMainMenuClick()
    {
        if (!IsOwner) return;
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void ToDesktopClick()
    {
        if (!IsOwner) return;
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        Application.Quit();
    }

    public float GetMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("sensitivity0", 500);
    }

    private string SensitivityFloatToString(float value)
    {
        string res = Mathf.FloorToInt(value).ToString();
        return res.Substring(0, Mathf.Min(res.Length, 4));
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
        if (!IsOwner) return;
        PlayerPrefs.SetFloat("volume", value);
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

    private void NetworkManager_OnClientDisconnectCallback(ulong clientid)
    {
        if (!IsOwner) return;
        if (clientid != 1) return;
        PerformanceLogger.gamePaused = true;
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientid)
    {
        if (clientid > 1)
            NetworkManager.Singleton.DisconnectClient(clientid);
        else if (clientid == 1)
        {
            waitForSecondPlayer = false;
        }

        //if (!IsOwner) return;
        Transform playerTransform = gameObject.transform.parent.GetComponentInParent<Transform>();
        playerTransform.position = spawnPositions.GetPosition(NetworkManager.Singleton.LocalClientId);
        playerTransform.rotation = spawnPositions.GetRotation(NetworkManager.Singleton.LocalClientId);
    }

    private void Awake()
    {
        spawnPositions = GameObject.Find("MainGame").GetComponent<SpawnPositions>();
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    private void Start()
    {
        if (!IsOwner) return;

        waitForSecondPlayer = true;
        gamePaused = true;
        PerformanceLogger.gamePaused = true;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        ShowHostWaitScreen();
    }

    void Update()
    {
        if (!IsOwner)
            return;

        if (waitForSecondPlayer && NetworkObject.OwnerClientId == 1)
        {
            waitForSecondPlayer = false;
            return;
        }

        if (waitForSecondPlayer)
        {
            return;
        }
        else if (hostWaitMenu.activeSelf && !waitForSecondPlayer)
        {
            gamePaused = false;
            PerformanceLogger.gamePaused = false;
            pauseCanvas.gameObject.SetActive(false);
            pauseCamera.gameObject.SetActive(false);
            hostWaitMenu.SetActive(false);
            controlsMenu.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        ChangeControlsLayout();

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                PerformanceLogger.gamePaused = true;
                playerCamera.gameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(true);
                pauseCamera.gameObject.SetActive(true);
                hostWaitMenu.SetActive(false);
                settingsMenu.SetActive(false);
                quitMenu.SetActive(false);
                controlsMenu.SetActive(false);
                mainMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                eventSystem.SetSelectedGameObject(selectedInMain);
            }
            else
            {
                PerformanceLogger.gamePaused = false;
                pauseCanvas.gameObject.SetActive(false);
                pauseCamera.gameObject.SetActive(false);
                playerCamera.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            if (!mainMenu.activeSelf)
            {
                BackToPauseMenuClick();
            }
            else
            {
                ResumeClick();
            }
        }
    }
}
