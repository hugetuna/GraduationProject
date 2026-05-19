using System;
using UnityEngine;
using UnityEngine.UI;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Computer"; // 這個字串用來指定動畫 key
    // 定義碰到電腦，按下互動鍵跳出選單的事件
    public static event Action OnComputerInteracted;
    //------------------------------------------------------------------//
    [Header("互動提示")]
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private GameObject desktopHint; // 與電腦互動時會出現的提示物件
    [SerializeField] private Button noButton; // 提示的 "否" 按鈕
    [SerializeField] private Button yesButton; // 提示的 "是" 按鈕
    //------------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip cancelSound; // 按下提示 "否" 按鈕的音效

    void Start()
    {
        desktopHint.SetActive(false);
        noButton.onClick.AddListener(CloseDesktopHint);
        yesButton.onClick.AddListener(InteractWithComputer);

        Invoke("UpdateColliderState", 1f); // 延遲 1 秒執行
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
        if (DayManager.Instance.dayEventManager.currentEvent.Type == EventType.WaitComputerOpen)
        {
            DayManager.Instance.dayEventManager.ComputerOpen();
        }
        OnComputerInteracted?.Invoke();
    }

    void IInteractable.Interact(int tool)
    {
        // 來自 IInteractable 介面
        if (CheckCanInteract())
        {
            desktopHint.SetActive(true); // 顯示提示物件
        }
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

    private void CloseDesktopHint()
    {
        AudioManager.Instance.PlaySFX(cancelSound);
        desktopHint.SetActive(false);
    }

    private void InteractWithComputer()
    {
        desktopHint.SetActive(false);
        
        TriggerOnComputerInteracted(); // 觸發電腦互動事件
        
        // 隱藏互動提示（減少提示物件與電腦介面交叉的不適感）
        if (transform.childCount > 0) 
        {
            transform.GetChild(0).gameObject.SetActive(false); 
        }
    }
}
