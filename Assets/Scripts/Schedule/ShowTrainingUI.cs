using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTrainingUI : MonoBehaviour, IInteractable
{
    // 定義碰到門，按下互動鍵跳出 UI 的事件
    public static event Action OnShowTrainingUI;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void IInteractable.Interact(int tool){ // 來自 IInteractable 介面
        Debug.Log("開啟訓練 UI");
        OnShowTrainingUI?.Invoke();
    }
}
