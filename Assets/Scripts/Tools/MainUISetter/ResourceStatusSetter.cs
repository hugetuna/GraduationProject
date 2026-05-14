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
        if(DayManager.Instance.chapter==0)
        {
            DayCountDown.text = $"距離下次公演剩餘{3 - DayManager.Instance.date}天";
            Date.text = $"2025/09/{DayManager.Instance.date + 21}";
        }
        else if(DayManager.Instance.chapter==1)
        {
            DayCountDown.text = $"距離下次公演剩餘{13 - DayManager.Instance.date}天";
            if(DayManager.Instance.date < 6)
            {
                Date.text = $"2025/9/{DayManager.Instance.date+24}";
            }
            else
            {
                Date.text = $"2025/10/{DayManager.Instance.date}";
            }
        }
        
    }
}
