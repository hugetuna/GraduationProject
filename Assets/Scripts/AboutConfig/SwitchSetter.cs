using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
public class SwitchSetter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI switchText;
    //元物件與位置
    public bool isMoved;
    private Vector3 originalPosition;
    public Vector3 hoverOffset = new Vector3(0, 10, 0); // 往上浮的距離
    void Start()
    {
        //紀錄位置
        originalPosition = transform.localPosition;
        if(isMoved)
        {
            SetPos(true);
        }
    }
    public void SetSwitchText(bool isOn)
    {
        if (isOn)
        {
            switchText.fontSize =48;
            //switchText.fontStyle = FontStyles.Bold;
        }
        else
        {
            switchText.fontSize = 36;
            switchText.fontStyle = FontStyles.Normal;
        }
    }
    public void SetPos(bool isOn)
    {
        if (isOn)
        {
            isMoved = true;
            transform.localPosition = originalPosition + hoverOffset;
        }
        else
        {
            isMoved = false;
            transform.localPosition = originalPosition;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isMoved) return;
        transform.localPosition = originalPosition + hoverOffset;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMoved) return;
        transform.localPosition = originalPosition;
    }

}
