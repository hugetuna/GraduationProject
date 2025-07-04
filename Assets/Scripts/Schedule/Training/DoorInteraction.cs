using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    // 定義碰到門，按下互動鍵跳出 UI 的事件
    public static event Action OnDoorInteracted;

    // void Start()
    // {
    // }

    // void Update()
    // {
        
    // }

    void IInteractable.Interact(int tool){ // 來自 IInteractable 介面
        if(!ScheduleManager.isSettled) OnDoorInteracted?.Invoke();
        else {
            // 今日的訓練結算後，不得再開啟訓練 UI（暫定）
            Debug.Log("訓練已結算，無法再更動成員");
        }   
    }
}
