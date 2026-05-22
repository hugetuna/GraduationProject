using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在 UIManager 上 */
public class GoOutUIHandler : MonoBehaviour
{
    [Header("外出 UI 整體")]
    [SerializeField] private GameObject goOutUI; // 整個外出介面
    //-----------------------------------------------------------------//
    [Header("UI 元素")]
    [SerializeField] private GameObject selectionUI; // 外出選擇介面
    [SerializeField] private Button closeButton; // 關閉整個外出介面的按鈕
    public static event Action OnBaitoOrActivityUIClosed; // 定義外出打工或商演介面關閉事件
    //-----------------------------------------------------------------//
    [SerializeField] private Button baitoButton; // 打工按鈕
    [SerializeField] private Button activityButton; // 商演按鈕
    private Activity appointedActivity;
    private TextMeshProUGUI activityBtnText;
    [SerializeField] private GameObject baitoUI; // 外出打工介面
    [SerializeField] private GameObject activityUI; // 外出商演介面
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟介面的音效
    [SerializeField] private AudioClip cancelSound; // 關閉外出介面的音效
    //-----------------------------------------------------------------//
    [Header("簡易新手教學")]
    [SerializeField] private ActivityTutorial activityTutorial; // 商演提示腳本

    void Start()
    {
        GoOutInteraction.OnExitInteracted += ShowSelectionUI; // 訂閱出門事件
        OnBaitoOrActivityUIClosed += RecallSelectionUI; // 訂閱打工或商演介面關閉事件

        closeButton.onClick.AddListener(CloseGoOutUI); // 為關閉按鈕添加點擊事件
        baitoButton.onClick.AddListener(ShowBaitoUI); // 為打工按鈕添加點擊事件
        activityButton.onClick.AddListener(ShowActivityUI); // 為商演按鈕添加點擊事件

        goOutUI.SetActive(false); // 預設隱藏整個外出介面

        activityBtnText = activityButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnDestroy()
    {
        GoOutInteraction.OnExitInteracted -= ShowSelectionUI; // 取消訂閱出門事件
        OnBaitoOrActivityUIClosed -= RecallSelectionUI; // 取消訂閱打工或商演介面關閉事件
    }

    private void ShowSelectionUI()
    {
        Debug.Log("開啟外出 UI");
        AudioManager.Instance.PlaySFX(openSound);
        UIAndPlayerInput.DisableAllPlayerInputs(); // 禁用角色移動
        goOutUI.SetActive(true);
        selectionUI.SetActive(true);
        baitoUI.SetActive(false);
        activityUI.SetActive(false);

        // 判斷是否啟用新手教學提示
        if(DayManager.Instance.totalDays == 1)
        {
            activityTutorial.ShowTutorial();
        }

        // 根據預約紀錄決定是否啟用商演按鈕
        appointedActivity = GameManager.Instance.activitySaveData.GetTodayActivity();
        if(appointedActivity != null)
        {
            activityButton.interactable = true;
            activityBtnText.color = Color.white;
        }
        else
        {
            activityButton.interactable = false;
            activityBtnText.color = new Color32(200, 200, 200, 255);
        }
    }

    private void RecallSelectionUI()
    {
        Debug.Log("返回選擇介面");
        selectionUI.SetActive(true);
        // 介面自行處理
        // baitoUI.SetActive(false); 
        // activityUI.SetActive(false);
    }

    private void CloseGoOutUI()
    {
        Debug.Log("關閉外出 UI");
        AudioManager.Instance.PlaySFX(cancelSound);
        UIAndPlayerInput.EnableAllPlayerInputs(); // 啟用角色移動
        goOutUI.SetActive(false);
    }

    private void ShowBaitoUI()
    {
        Debug.Log("選擇打工");
        AudioManager.Instance.PlaySFX(openSound);
        selectionUI.SetActive(false);
        baitoUI.SetActive(true);
        baitoUI.GetComponent<SetBaitoUI>().OpenBaitoUI();
        activityUI.SetActive(false);
    }

    private void ShowActivityUI()
    {
        Debug.Log("選擇商演");
        AudioManager.Instance.PlaySFX(openSound);
        selectionUI.SetActive(false);
        baitoUI.SetActive(false);
        activityUI.SetActive(true);
        activityUI.GetComponent<SetActivityUI>().OpenActivityUI(appointedActivity);
    }

    public static void TriggerUIsClosedEvent()
    {
        OnBaitoOrActivityUIClosed?.Invoke();
    }
}
