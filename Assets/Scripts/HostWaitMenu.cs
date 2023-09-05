using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostWaitMenu : MonoBehaviour
{

    public void ClickBack()
    {
        Debug.Log("back click");
        SceneManager.LoadScene("MainMenu");
        NetworkManager.Singleton.Shutdown();
        SceneManager.UnloadSceneAsync("TestMP");
    }
    void Start()
    {
        
    }

    void Update()
    {

    }
}
