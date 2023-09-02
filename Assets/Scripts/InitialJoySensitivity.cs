using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialJoySensitivity : MonoBehaviour
{
    void Start()
    {
        Slider slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat("jsensitivity", 500f);
    }
    void Update()
    {
        
    }
}
