using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSizeIndicator : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public Color activeColor;
    public Color inactiveColor;

    public Image smallImage;
    public Image mediumImage;
    public Image largeImage;

    void Start()
    {
        
    }

    void Update()
    {
        switch (playerMovement.curSize)
        {
            case 0:
                smallImage.color = activeColor;
                mediumImage.color = inactiveColor;
                largeImage.color = inactiveColor;
                break;
            case 1:
                smallImage.color = inactiveColor;
                mediumImage.color = activeColor;
                largeImage.color = inactiveColor;
                break;
             case 2:
                smallImage.color = inactiveColor;
                mediumImage.color = inactiveColor;
                largeImage.color = activeColor;
                break;
        }
    }
}
