using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelMP : NetworkBehaviour
{
    private int playersInTrigger;

    private void Start()
    {
        playersInTrigger = 0;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player"))
        {
            return;
        }

        playersInTrigger++;
    }

    private void OnTriggerExit(Collider other)
    {

        if (!other.CompareTag("Player"))
        {
            return;
        }

        playersInTrigger--;
    }

    private void Update()
    {
        if (playersInTrigger < 2)
        {
            return;
        }

        PerformanceLogger.gamePaused = true;
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);

        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.Confined;
    }
}
