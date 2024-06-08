using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SSPause : MonoBehaviour
{
    public bool paused;
    [SerializeField]
    private PlayerLook playerLook;
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private GameObject canvasGameObject;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private PlayerInput playerInput;

    [SerializeField]
    private TMP_InputField sensitivityInputField;
    [SerializeField]
    private Slider sensitivitySlider;
    [SerializeField]
    private Slider volumeSlider;

    private readonly Vector3 uiOffset = new Vector3(240, 0, 0);

    private void DisableChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.name == "Background")
            {
                continue;
            }
            child.gameObject.SetActive(false);
        }
    }

    private void ApplyPositionOffset(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (playerInput.playerIndex == 0)
            {
                child.gameObject.GetComponent<RectTransform>().anchoredPosition3D -= uiOffset;
            }
            else
            {
                child.gameObject.GetComponent<RectTransform>().anchoredPosition3D += uiOffset;
            }
        }
    }

    private void Resume()
    {
        canvasGameObject.SetActive(false);
        playerLook.x = 0;
        playerLook.y = 0;
        playerLook.enabled = true;
        paused = false;
        playerInput.defaultActionMap = "Player";
    }

    public void ResumeCick()
    {
        Resume();
    }

    public void SettingsClick()
    {
        DisableChildren(canvasGameObject);
        GameObject settingsMenu = GetChildByName.Get(canvasGameObject, "SettingsMenu");
        settingsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(GetChildByName.Get(settingsMenu, "BackButton"));
    }
    private float GetVolume()
    {
        return PlayerPrefs.GetFloat("volume", 0.5f);
    }
    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
    }

    public float GetSensitivity()
    {
        return PlayerPrefs.GetFloat("sensitivity" + playerInput.playerIndex, 500);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("sensitivity" + playerInput.playerIndex, value);
        string valueString = Mathf.FloorToInt(value).ToString();
        valueString = valueString.Substring(0, Mathf.Min(valueString.Length, 4));
        sensitivityInputField.text = valueString;
    }
    public void SetSensitivity(string value)
    {
        float floatValue = float.Parse(value);
        PlayerPrefs.SetFloat("sensitivity" + playerInput.playerIndex, floatValue);
        sensitivitySlider.value = floatValue;
    }

    public void BackClick()
    {
        DisableChildren(canvasGameObject);
        GameObject pauseMenu = GetChildByName.Get(canvasGameObject, "PauseMenu");
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(GetChildByName.Get(pauseMenu, "ResumeButton"));
    }

    public void QuitClick()
    {
        DisableChildren(canvasGameObject);
        GameObject quitMenu = GetChildByName.Get(canvasGameObject, "QuitMenu");
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(GetChildByName.Get(quitMenu, "CancelButton"));
    }

    public void ToMainMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ToDesktopClick()
    {
        Application.Quit();
    }

    private void OnGamePause()
    {
        paused = !paused;
        if (!paused)
        {
            Resume();
        }
        else
        {
            playerInput.defaultActionMap = "UI";
            playerLook.enabled = false;
            canvasGameObject.SetActive(true);
            playerMovement.rawInput = Vector3.zero;
            playerMovement.normalizedInput = Vector3.zero;
            playerMovement.clampedInput = Vector3.zero;
            BackClick();
        }
    }

    private void OnCancel()
    {
        if (!paused)
        {
            return;
        }

        if (GetChildByName.Get(canvasGameObject, "PauseMenu").activeSelf)
        {
            Resume();
        }
        else
        {
            DisableChildren(canvasGameObject);
            GameObject pauseMenu = GetChildByName.Get(canvasGameObject, "PauseMenu");
            pauseMenu.SetActive(true);
            eventSystem.SetSelectedGameObject(GetChildByName.Get(pauseMenu, "ResumeButton"));
        }

    }
    void Start()
    {
        paused = false;
        ApplyPositionOffset(canvasGameObject);
        playerInput.defaultActionMap = "Player";
    }

    void Update()
    {
        GameObject settingsMenu = GetChildByName.Get(canvasGameObject, "SettingsMenu");
        if (settingsMenu.activeSelf)
        {
            volumeSlider.value = GetVolume();
        }
    }
}
