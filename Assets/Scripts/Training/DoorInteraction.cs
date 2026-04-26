using System;
using UnityEngine;

/* 掛在訓練室的門（玩家互動對象）上 */
public class DoorInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Training"; // 這個字串用來指定動畫 key
    //-----------------------------------------------------------------//
    [SerializeField] private TrainingUIData trainingUIData;
    public static event Action<TrainingUIData> OnDoorInteracted; // 定義碰到門，按下互動鍵跳出 UI 的事件

    void IInteractable.Interact(int tool)
    { // 來自 IInteractable 介面
        OnDoorInteracted?.Invoke(trainingUIData); // 觸發事件
    }
}
