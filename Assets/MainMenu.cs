using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject singelplayerMenu;
    public void SingePlayerClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        singelplayerMenu.SetActive(true);
    }

    public void SettingsClick()
    {
        mainMenu.SetActive(false);
        singelplayerMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void QuitClick()
    {
        Application.Quit();
    }

    public void BackToMainMenuClick()
    {
        singelplayerMenu.SetActive(false);
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
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
}
