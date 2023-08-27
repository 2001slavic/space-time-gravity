using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool gamePaused;
    public GameObject mainGameObject;
    public Canvas pauseCanvas;
    public Camera pauseCamera;

    public GameObject mainMenu;
    public GameObject settingsMenu;

    public MouseLook mouseLook;

    public void ResumeClick()
    {
        gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        mainGameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.mouseSensitivity = GetMouseSensitivity();
    }

    public void SettingsClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitClick()
    {
        Application.Quit();
    }

    public void BackToMainMenuClick()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public float GetMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("sensitivity");
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("sensitivity", value);
    }
    void Start()
    {
        gamePaused = false;
        pauseCanvas.gameObject.SetActive(false);
        pauseCamera.gameObject.SetActive(false);
        mainGameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook.mouseSensitivity = GetMouseSensitivity();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                mainGameObject.SetActive(false);
                pauseCanvas.gameObject.SetActive(true);
                pauseCamera.gameObject.SetActive(true);
                settingsMenu.SetActive(false);
                mainMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                pauseCanvas.gameObject.SetActive(false);
                pauseCamera.gameObject.SetActive(false);
                mainGameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                mouseLook.mouseSensitivity = GetMouseSensitivity();
            }
        }
    }
}
