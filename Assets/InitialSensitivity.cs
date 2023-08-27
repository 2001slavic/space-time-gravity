using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialSensitivity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Slider slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat("sensitivity");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
