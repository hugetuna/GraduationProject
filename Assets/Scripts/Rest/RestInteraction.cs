using System;
using UnityEngine;

public class RestInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Rest"; // 這個字串用來指定動畫 key
    //-----------------------------------------------------------------//
    public static event Action OnRestInteracted; // 定義碰到門，按下互動鍵跳出 UI 的事件
    [SerializeField] private BoxCollider myBoxCollider;

    void Start()
    {
        Invoke("UpdateColliderState", 1f); // 延遲 1 秒執行
    }

    private void UpdateColliderState()
    {
        if (CheckCanInteract())
        {
            // Debug.Log("休息室互動提示已啟用");
            myBoxCollider.enabled = true; // 啟用休息室互動
        }
        else
        {
            // Debug.Log("休息室互動提示已禁用");
            myBoxCollider.enabled = false; // 禁用休息室互動
        }
    }

    void IInteractable.Interact(int tool) // 來自 IInteractable 介面
    {
        if (CheckCanInteract()) OnRestInteracted?.Invoke(); // 觸發事件
    }

    private bool CheckCanInteract()
    {
        if (DayManager.Instance == null || DayManager.Instance.dayEventManager.currentEvent == null)
        {
            return true;
        }

        var currentEvent = DayManager.Instance.dayEventManager.currentEvent;
        int chapter = DayManager.Instance.chapter;
        int date = DayManager.Instance.date;

        if (date == 1 && chapter == 0 && currentEvent.TriggerTimeIndex < 10)
        {
            Debug.Log("完成特定第一天事件前不觸發");
            return false;
        }
        if (date == 2 && chapter == 0 && currentEvent.TriggerTimeIndex < 10)
        {
            Debug.Log("完成特定第二天事件前不觸發");
            return false;
        }

        return true;
    }
}
