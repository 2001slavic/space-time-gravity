using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool gamePaused;
    public GameObject mainGameObject;
    public Canvas pauseCanvas;
    public Camera pauseCamera;

    public EventSystem eventSystem;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    [SerializeField]
    private GameObject controlsMenu;
    public GameObject quitMenu;

    public GameObject selectedInMain;
    public GameObject selectedInSettings;
    public GameObject selectedInQuit;

    [SerializeField]
    private PlayerLook playerLook;
    [SerializeField]
    private TMP_InputField sensitivityInputField;
    [SerializeField]
    private Slider sensitivitySlider;

    public AudioSource[] audioSources;

    
    private string lastUsedInputDevice;

    private string SensitivityFloatToString(float value)
    {
        string res = Mathf.FloorToInt(value).ToString();
        return res.Substring(0, Mathf.Min(res.Length, 4));
    }

    public void ResumeClick()
    {
        gamePaused = false;
        PerformanceLogger.gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        mainGameObject.SetActive(true);
        playerLook.x = 0;
        playerLook.y = 0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SettingsClick()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(true);
        sensitivityInputField.text = SensitivityFloatToString(GetMouseSensitivity());
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void ControlsClick()
    {
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void QuitMenuClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void BackToPauseMenuClick()
    {
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInMain);
    }

    public void ToMainMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
        SceneManager.UnloadSceneAsync("SP1");
    }

    public void ToDesktopClick()
    {
        Application.Quit();
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

    private void ResetAudioClips()
    {
        foreach (AudioSource audioSource in audioSources)
            audioSource.clip = null;
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

    void Start()
    {
        gamePaused = false;
        PerformanceLogger.gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        mainGameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        lastUsedInputDevice = "Keyboard";
    }
    void Update()
    {
        ChangeControlsLayout();
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                PerformanceLogger.gamePaused = true;
                ResetAudioClips();
                mainGameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(true);
                pauseCamera.gameObject.SetActive(true);
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
                mainGameObject.SetActive(true);
                playerLook.x = 0;
                playerLook.y = 0;
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
