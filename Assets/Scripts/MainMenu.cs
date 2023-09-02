using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public EventSystem eventSystem;

    public GameObject mainMenu;
    public GameObject settingsMenu;
    public GameObject singelplayerMenu;
    //public GameObject multiPlayerMenu;
    public GameObject networkMPMenu;
    public GameObject quitMenu;

    public GameObject selectedInMainMenu;
    public GameObject selectedInSettings;
    public GameObject selectedInSP;
    public GameObject selectedInQuit;
    public GameObject selectedInNetworkMP;

    public void NewScene(string name)
    {
        SceneManager.LoadScene(name);

        if (SceneManager.GetActiveScene().name != name)
        {
            StartCoroutine("WaitForSceneLoad", name);
        }
    }

    IEnumerator WaitForSceneLoad(string name)
    {
        while (SceneManager.GetActiveScene().name != name)
        {
            yield return null;
        }

        //// Do anything after proper scene has been loaded
        //if (SceneManager.GetActiveScene().name == name)
        //{
        //    Debug.Log(SceneManager.GetActiveScene().buildIndex);
        //    GameObject spawner = GameObject.FindWithTag("scene" + sceneNumber);
        //    spawner.GetComponent<spawnPlayer>().spawn(team);
        //}
        //currentScene = sceneNumber;
    }
    public void SingePlayerClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        quitMenu.SetActive(false);
        //multiPlayerMenu.SetActive(false);
        networkMPMenu.SetActive(false);
        singelplayerMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSP);
    }

    public void SettingsClick()
    {
        mainMenu.SetActive(false);
        singelplayerMenu.SetActive(false);
        quitMenu.SetActive(false);
        //multiPlayerMenu.SetActive(false);
        networkMPMenu.SetActive(false);
        settingsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInSettings);
    }

    public void QuitMenuClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        singelplayerMenu.SetActive(false);
        //multiPlayerMenu.SetActive(false);
        networkMPMenu.SetActive(false);
        quitMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(selectedInQuit);
    }

    public void NetworkMPMenuClick()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        singelplayerMenu.SetActive(false);
        //multiPlayerMenu.SetActive(false);
        quitMenu.SetActive(false);
        networkMPMenu.SetActive(true);
        
        eventSystem.SetSelectedGameObject(selectedInNetworkMP);
    }

    public void HostClick()
    {
        SceneManager.LoadSceneAsync("TestMP");
        while (SceneManager.GetActiveScene().name != "TestMP")
            ;

        NetworkManager.Singleton.StartHost();
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void ClientClick()
    {
        SceneManager.LoadSceneAsync("TestMP");
        while (SceneManager.GetActiveScene().name != "TestMP")
            ;

        NetworkManager.Singleton.StartClient();
        SceneManager.UnloadSceneAsync("MainMenu");
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
        //multiPlayerMenu.SetActive(false);
        networkMPMenu.SetActive(false);
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
        //multiPlayerMenu.SetActive(false);
        networkMPMenu.SetActive(false);
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
