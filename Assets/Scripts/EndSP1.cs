using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSP1 : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PerformanceLogger.gamePaused = true;
            SceneManager.LoadScene("MainMenu");
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
