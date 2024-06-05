using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SSPause : MonoBehaviour
{
    public bool paused;
    [SerializeField]
    private PlayerLook playerLook;
    [SerializeField]
    private GameObject canvasGameObject;
    [SerializeField]
    private EventSystem eventSystem;

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

    private void Resume()
    {
        canvasGameObject.SetActive(false);
        playerLook.x = 0;
        playerLook.y = 0;
        playerLook.enabled = true;
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
    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
    }

    public void BackClick()
    {
        DisableChildren(canvasGameObject);
        GameObject pauseMenu = GetChildByName.Get(canvasGameObject, "PauseMenu");
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(GetChildByName.Get(pauseMenu, "ResumeButton"));
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
            Debug.Log()
            playerLook.enabled = false;
            canvasGameObject.SetActive(true);
            BackClick();
        }
    }
    void Start()
    {
        paused = false;
    }

    void Update()
    {
        
    }
}
