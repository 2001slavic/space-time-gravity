using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCurrentCamera : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject topCamera;
    public SkinnedMeshRenderer robotRender;
    // Start is called before the first frame update
    void Start()
    {
        if (topCamera == null)
            return;
        playerCamera.SetActive(true);
        topCamera.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (topCamera == null)
            return;
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            playerCamera.SetActive(!playerCamera.activeSelf);
            topCamera.SetActive(!topCamera.activeSelf);
        }
        robotRender.enabled = !playerCamera.activeSelf;
    }
}
