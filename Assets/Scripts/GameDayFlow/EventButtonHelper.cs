using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventButtonHelper : MonoBehaviour
{
    public string ButtonName;
    public void NotifyManager()
    {
        DayManager.Instance?.dayEventManager.PressEventButton(ButtonName);
    }
}
