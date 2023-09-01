using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialVolume : MonoBehaviour
{
    void Start()
    {
        Slider slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat("volume", 0.5f);
    }

    void Update()
    {
        
    }
}
