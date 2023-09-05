using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseNetwork : NetworkBehaviour
{
    public bool gamePaused;
    public Canvas pauseCanvas;
    public Camera pauseCamera;
    public Camera playerCamera;

    public EventSystem eventSystem;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject quitMenu;
    public GameObject hostWaitMenu;

    public GameObject selectedInMain;
    public GameObject selectedInSettings;
    public GameObject selectedInQuit;
    public GameObject selectedInHostWait;

    public MouseLookNetwork mouseLook;
    public JoystickLookNetwork joystickLook;

    public bool waitForSecondPlayer;

    public void ShowHostWaitScreen()
    {
        if (!IsOwner) return;
        playerCamera.gameObject.SetActive(false);
        pauseCanvas.gameObject.SetActive(true);
        pauseCamera.gameObject.SetActive(true);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        mainMenu.SetActive(false);
        hostWaitMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        eventSystem.SetSelectedGameObject(selectedInHostWait);
    }

    public void ResumeClick()
    {
        if (!IsOwner) return;
        gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.mouseSensitivity = GetMouseSensitivity();
        joystickLook.joyLookSensitivity = GetJoystickLookSensitivity();
    }

    public void SettingsClick()
    {
        if (!IsOwner) return;
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        settingsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void QuitMenuClick()
    {
        if (!IsOwner) return;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void BackToPauseMenuClick()
    {
        if (!IsOwner) return;
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInMain);
    }

    public void ToMainMenuClick()
    {
        if (!IsOwner) return;
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu");
        SceneManager.UnloadSceneAsync("TestMP");
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
        return PlayerPrefs.GetFloat("sensitivity", 500);
    }

    public void SetMouseSensitivity(float value)
    {
        if (!IsOwner) return;
        PlayerPrefs.SetFloat("sensitivity", value);
    }

    public float GetJoystickLookSensitivity()
    {
        return PlayerPrefs.GetFloat("jsensitivity", 500);
    }

    public void SetJoystickSensitivity(float value)
    {
        if (!IsOwner) return;
        PlayerPrefs.SetFloat("jsensitivity", value);
    }

    public void SetVolume(float value)
    {
        if (!IsOwner) return;
        PlayerPrefs.SetFloat("volume", value);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientid)
    {
        if (!IsOwner) return;
        if (clientid != 1) return;
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.UnloadSceneAsync("TestMP");
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientid)
    {
        if (clientid > 1)
            NetworkManager.Singleton.DisconnectClient(clientid);
        else if (clientid == 1)
        {
            waitForSecondPlayer = false;
        }
    }


    void Start()
    {
        if (!IsOwner) return;
        waitForSecondPlayer = true;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        gamePaused = true;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        hostWaitMenu.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.mouseSensitivity = GetMouseSensitivity();
        joystickLook.joyLookSensitivity = GetJoystickLookSensitivity();
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
            pauseCanvas.gameObject.SetActive(false);
            pauseCamera.gameObject.SetActive(false);
            hostWaitMenu.SetActive(false);
            playerCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            mouseLook.mouseSensitivity = GetMouseSensitivity();
            joystickLook.joyLookSensitivity = GetJoystickLookSensitivity();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                playerCamera.gameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(true);
                pauseCamera.gameObject.SetActive(true);
                hostWaitMenu.SetActive(false);
                settingsMenu.SetActive(false);
                quitMenu.SetActive(false);
                mainMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                eventSystem.SetSelectedGameObject(selectedInMain);
            }
            else
            {
                pauseCanvas.gameObject.SetActive(false);
                pauseCamera.gameObject.SetActive(false);
                playerCamera.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                mouseLook.mouseSensitivity = GetMouseSensitivity();
                joystickLook.joyLookSensitivity = GetJoystickLookSensitivity();
            }
        }
        else if (Input.GetButtonDown("Cancel") && !mainMenu.activeSelf)
        {
            BackToPauseMenuClick();
        }
    }
}
