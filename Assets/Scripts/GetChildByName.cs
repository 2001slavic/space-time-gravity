using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetChildByName
{
    public static GameObject Get(GameObject parent, string name)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.name == name)
                return child.gameObject;
        }

        return null;
    }
}
