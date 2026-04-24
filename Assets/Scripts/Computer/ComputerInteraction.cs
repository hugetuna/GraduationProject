using System;
using UnityEngine;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Computer"; // 這個字串用來指定動畫 key
    // 定義碰到電腦，按下互動鍵跳出選單的事件
    public static event Action OnComputerInteracted;
    [SerializeField] private BoxCollider boxCollider;

    void Start()
    {
        InvokeRepeating("UpdateColliderState", 1f, 300f); // 第一次延遲 1 秒，之後每五分鐘檢查一次
    }

    private void UpdateColliderState()
    {
        if (CheckCanInteract())
        {
            // Debug.Log("電腦互動提示已啟用");
            boxCollider.enabled = true; // 啟用電腦互動
        }
        else
        {
            // Debug.Log("電腦互動提示已禁用");
            boxCollider.enabled = false; // 禁用電腦互動
        }
    }

    // 讓外部（如轉場管理器）可以直接調用，不需要實體
    public static void TriggerOnComputerInteracted()
    {
        Debug.Log($"電腦互動事件被觸發{OnComputerInteracted != null}");
        OnComputerInteracted?.Invoke();
    }
    void IInteractable.Interact(int tool)
    {
        // 來自 IInteractable 介面
        if (CheckCanInteract())
        {
            OnComputerInteracted?.Invoke();
            if (transform.childCount > 0)
            {
                // 隱藏互動提示（減少提示物件與電腦介面交叉的不適感）
                transform.GetChild(0).gameObject.SetActive(false); 
            }
        }
    }

    private bool CheckCanInteract()
    {
        if (DayManager.Instance == null || DayManager.Instance.dayEventManager.currentEvent == null)
        {
            return true;
        }

        var currentEvent = DayManager.Instance.dayEventManager.currentEvent;
        int date = DayManager.Instance.date;

        if (date == 1 && currentEvent.TriggerTimeIndex < 14)
        {
            Debug.Log("完成特定第一天事件前不觸發");
            return false;
        }

        if (date == 2 && currentEvent.TriggerTimeIndex < 10)
        {
            Debug.Log("完成特定第二天事件前不觸發");
            return false;
        }

        return true;
    }
}
