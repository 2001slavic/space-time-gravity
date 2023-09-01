using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialSensitivity : MonoBehaviour
{
    void Start()
    {
        Slider slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat("sensitivity", 500f);
    }

    void Update()
    {
        
    }
}
