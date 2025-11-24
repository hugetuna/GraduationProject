using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Computer"; // 這個字串用來指定動畫 key
    // 定義碰到電腦，按下互動鍵跳出選單的事件
    public static event Action OnComputerInteracted;

    // void Start()
    // {
    // }

    // void Update()
    // {
        
    // }

    void IInteractable.Interact(int tool){ // 來自 IInteractable 介面
        if (DayManager.Instance.date == 1 && DayManager.Instance.dayEventManager.EventedNumberToday <= 13)
        {
            Debug.Log("第一天特定事件前不觸發電腦互動");
            return; // 第一天在特定事件前不觸發互動
        }
        OnComputerInteracted?.Invoke();  
    }
}
