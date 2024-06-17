using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelSS : MonoBehaviour
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

        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.Confined;
    }
}
