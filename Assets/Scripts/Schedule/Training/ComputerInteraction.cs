using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    // 定義碰到電腦，按下互動鍵跳出選單的事件
    public static event Action OnComputerInteracted;

    // void Start()
    // {
    // }

    // void Update()
    // {
        
    // }

    void IInteractable.Interact(int tool){ // 來自 IInteractable 介面
        OnComputerInteracted?.Invoke();  
    }
}
