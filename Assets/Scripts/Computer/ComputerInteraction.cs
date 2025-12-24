using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Computer"; // 這個字串用來指定動畫 key
    // 定義碰到電腦，按下互動鍵跳出選單的事件
    public static event Action OnComputerInteracted;
    void IInteractable.Interact(int tool)
    { 
        // 來自 IInteractable 介面
        if (FindObjectsByType<DayManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0)
        {
            if (DayManager.Instance.dayEventManager.currentEvent.TriggerTimeIndex <14&&DayManager.Instance.date==1)
            {
                Debug.Log("完成特定第一天事件前不觸發");
                return;
            }
            if (DayManager.Instance.dayEventManager.currentEvent.TriggerTimeIndex < 10 && DayManager.Instance.date == 2)
            {
                Debug.Log("完成特定第二天事件前不觸發");
                return;
            }
        }
        OnComputerInteracted?.Invoke();
    }
}
