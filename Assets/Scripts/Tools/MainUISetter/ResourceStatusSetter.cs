using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ResourceStatusSetter : MonoBehaviour
{
    public TextMeshProUGUI Date;
    public TextMeshProUGUI DayCountDown;
    public TextMeshProUGUI MoneyCount;
    public Image WeatherIcon;
    public void setByResourceAndDay()
    {
        MoneyCount.text = ResourceManager.Instance.Money.ToString();
        DayCountDown.text = $"剩餘天數{(3 - DayManager.Instance.date)}天";
        Date.text = $"2025/09/{DayManager.Instance.date+21}";
    }
}
