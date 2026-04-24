using System;
using UnityEngine;

/* 掛在一樓出口（玩家互動對象）上 */
public class GoOutInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Gate"; // 這個字串用來指定動畫 key
    //-----------------------------------------------------------------//
    public static event Action OnExitInteracted; // 定義在出口範圍內，按下互動鍵跳出 UI 的事件

    void IInteractable.Interact(int tool)
    { 
        // 來自 IInteractable 介面
        OnExitInteracted?.Invoke(); // 觸發事件
    }
}
