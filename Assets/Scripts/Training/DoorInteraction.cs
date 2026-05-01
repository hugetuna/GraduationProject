using System;
using UnityEngine;

/* 掛在訓練室的門（玩家互動對象）上 */
public class DoorInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Training"; // 這個字串用來指定動畫 key
    //-----------------------------------------------------------------//
    [SerializeField] private TrainingUIData trainingUIData;
    public static event Action<TrainingUIData> OnDoorInteracted; // 定義碰到門，按下互動鍵跳出 UI 的事件
    [SerializeField] private GameObject firstDayHintObj; // 第一天專用的提示物件，防止玩家撤銷指定角色的訓練
    private OnlyOneTraineeHint firstDayHintComp; // 方便存取用


    void Start()
    {
        if (firstDayHintObj != null)
        {
            firstDayHintComp = firstDayHintObj.GetComponent<OnlyOneTraineeHint>();
            firstDayHintObj.SetActive(false);
        }
    }

    void IInteractable.Interact(int tool) // 來自 IInteractable 介面
    {
        if (IsFirstDayTrainingConstraint())
        {
            ShowFirstDayHint(); // 顯示提示物件
            return; // 不繼續執行打開訓練 UI 的邏輯
        }
        OnDoorInteracted?.Invoke(trainingUIData); // 觸發事件
    }

    private bool IsFirstDayTrainingConstraint()
    {
        if (DayManager.Instance == null || DayManager.Instance.dayEventManager.currentEvent == null) return false;

        int date = DayManager.Instance.date;
        int eventIndex = DayManager.Instance.dayEventManager.currentEvent.TriggerTimeIndex;

        // 目前第一天的事件 6 是指定角色訓練，而事件 7 是繼續逛下一層樓
        return date == 1 && eventIndex > 6;
    }

    private void ShowFirstDayHint()
    {
        if (firstDayHintObj != null && firstDayHintComp != null)
        {
            firstDayHintObj.SetActive(true);
            firstDayHintComp.SetHintUI();
            UIAndPlayerInput.DisableAllPlayerInputs();
        }
    }
}
