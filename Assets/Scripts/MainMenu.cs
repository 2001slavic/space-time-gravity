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

    private const int UNDEFINED = 0;
    private const int HOST = 1;
    private const int CLIENT = 2;

    private int networkManagerQuery;


    // A public method that can be called from other scripts or UI buttons
    private void LoadScene(string sceneName)
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the scene normally
        SceneManager.LoadScene(sceneName);
    }

    // A private method that will be called when a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Optionally, you can do some additional logic here after the scene is loaded
        switch (networkManagerQuery)
        {
            case HOST:
                NetworkManager.Singleton.StartHost();
                break;
            case CLIENT:
                NetworkManager.Singleton.StartClient();
                break;
        }
        //SceneManager.UnloadSceneAsync("MainMenu");
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
        networkManagerQuery = HOST;
        LoadScene("TestMP");
    }

    public void ClientClick()
    {
        networkManagerQuery = CLIENT;
        LoadScene("TestMP");
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
        networkManagerQuery = UNDEFINED;
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
