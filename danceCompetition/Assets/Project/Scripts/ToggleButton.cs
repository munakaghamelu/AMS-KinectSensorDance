using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    bool isActive = false;
    public Image background;

    public void OnChangeValue()
    {
        isActive = gameObject.GetComponent<Toggle>().isOn;
        if(isActive)
        {
            background.color = new Color32(183, 0, 0, 255);
        }
        else
        {
            background.color = new Color32(147, 103, 103, 255);
        }
    }
}
