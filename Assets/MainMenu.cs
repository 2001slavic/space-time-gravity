using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject singelplayerMenu;
    public GameObject quitMenu;

    public GameObject selectedInMainMenu;
    public GameObject selectedInSettings;
    public GameObject selectedInSP;
    public GameObject selectedInQuit;
    public void SingePlayerClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        singelplayerMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSP);
    }

    public void SettingsClick()
    {
        mainMenu.SetActive(false);
        singelplayerMenu.SetActive(false);
        quitMenu.SetActive(false);
        settingsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void QuitMenuClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        singelplayerMenu.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void QuitYesClick()
    {
        Application.Quit();
    }

    public void BackToMainMenuClick()
    {
        singelplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
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
        PlayerPrefs.SetFloat("sensitivity", value);
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
    }

    public void SetJoystickSensitivity(float value)
    {
        PlayerPrefs.SetFloat("jsensitivity", value);
    }

    private void Start()
    {
        singelplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (mainMenu.activeSelf)
            {
                QuitMenuClick();
            }
            else
            {
                BackToMainMenuClick();
            }
        }
    }
}
