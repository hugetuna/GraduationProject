using System;
using UnityEngine;

public class LoungeInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Rest"; // 這個字串用來指定動畫 key
    //-----------------------------------------------------------------//
    public static event Action OnLoungeInteracted; // 定義碰到門，按下互動鍵跳出 UI 的事件


    void IInteractable.Interact(int tool) // 來自 IInteractable 介面
    {
        OnLoungeInteracted?.Invoke(); // 觸發事件
    }
}
