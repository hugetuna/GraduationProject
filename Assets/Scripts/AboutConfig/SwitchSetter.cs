using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SwitchSetter : MonoBehaviour
{
    public TextMeshProUGUI switchText;
    public void SetSwitchText(bool isOn)
    {
        if (isOn)
        {
            switchText.fontSize =48;
            switchText.fontStyle = FontStyles.Bold;
        }
        else
        {
            switchText.fontSize = 36;
            switchText.fontStyle = FontStyles.Normal;
        }
    }
}
