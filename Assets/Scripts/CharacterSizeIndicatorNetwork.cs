using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSizeIndicatorNetwork : NetworkBehaviour
{
    [SerializeField]
    private SizeControlNetwork sizeControl;
    public Color activeColor;
    public Color inactiveColor;

    public Image smallImage;
    public Image mediumImage;
    public Image largeImage;

    void Start()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Update()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }

        switch (sizeControl.size)
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
