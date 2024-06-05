using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public void ResumeClick()
    {
        gamePaused = false;
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
        settingsMenu.SetActive(true);
        sensitivityInputField.text = GetMouseSensitivity().ToString();
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void QuitMenuClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void BackToPauseMenuClick()
    {
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
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
        return PlayerPrefs.GetFloat("sensitivity", 500);
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("sensitivity", value);
        string valueString = Mathf.FloorToInt(value).ToString();
        valueString = valueString.Substring(0, Mathf.Min(valueString.Length, 4));
        sensitivityInputField.text = valueString;
    }
    public void SetMouseSensitivity(string value)
    {
        float floatValue = float.Parse(value);
        PlayerPrefs.SetFloat("sensitivity", floatValue);
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
    void Start()
    {
        gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        mainGameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Start"))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                ResetAudioClips();
                mainGameObject.SetActive(false);
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
                mainGameObject.SetActive(true);
                playerLook.x = 0;
                playerLook.y = 0;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        else if (Input.GetButtonDown("Cancel") && !mainMenu.activeSelf)
        {
            BackToPauseMenuClick();
        }
    }
}
