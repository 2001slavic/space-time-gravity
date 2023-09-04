using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
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

    public GameObject selectedInMain;
    public GameObject selectedInSettings;
    public GameObject selectedInQuit;

    public MouseLookNetwork mouseLook;
    public JoystickLookNetwork joystickLook;

    public void ResumeClick()
    {
        if (!IsOwner) return;
        gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
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
        settingsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void QuitMenuClick()
    {
        if (!IsOwner) return;
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void BackToPauseMenuClick()
    {
        if (!IsOwner) return;
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInMain);
    }

    public void ToMainMenuClick()
    {
        if (!IsOwner) return;
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("MainMenu");
        SceneManager.UnloadSceneAsync("TestMP");
    }

    public void ToDesktopClick()
    {
        if (!IsOwner) return;
        NetworkManager.Singleton.Shutdown();
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
        SceneManager.LoadScene("MainMenu");
        SceneManager.UnloadSceneAsync("TestMP");
    }


    void Start()
    {
        if (!IsOwner) return;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.mouseSensitivity = GetMouseSensitivity();
        joystickLook.joyLookSensitivity = GetJoystickLookSensitivity();
    }
    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                playerCamera.gameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(true);
                pauseCamera.gameObject.SetActive(true);
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
