using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在 UIManager 上 */
public class ActivityUIHandler : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private GameObject activityUI; // 外出商演介面
    [SerializeField] private Button closeButton; // 關閉介面的按鈕
    [SerializeField] private TextMeshProUGUI nameText; // 商演名稱文字
    [SerializeField] private TextMeshProUGUI descriptionText; // 商演說明文字
    [SerializeField] private TextMeshProUGUI VigourCostText; // 商演耗體文字
    [SerializeField] private TextMeshProUGUI MoneyGainText; // 商演收益文字
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟介面的音效
    //-----------------------------------------------------------------//
    [Header("商演資訊")]
    [SerializeField] private Activity activityForTest; // 商演資料 SO（測試用，之後會從預約存檔中取得）

    void Start()
    {
        GoOutInteraction.OnExitInteracted += ShowActivityUI; // 訂閱出門事件
        closeButton.onClick.AddListener(CloseActivityUI); // 設定關閉按鈕的監聽事件
        // numbersController = trainingUI.GetComponent<NumbersController>();
    }

    void OnDestroy()
    {
        GoOutInteraction.OnExitInteracted -= ShowActivityUI; // 取消訂閱出門事件
    }

    public void ShowActivityUI()
    {
        Debug.Log("開啟外出商演 UI");
        activityUI.SetActive(true);
        UIAndPlayerInput.DisableAllPlayerInputs(); // 禁止角色走動
        if (openSound != null) AudioManager.Instance.PlaySFX(openSound);

        // 設定商演介面的基本資訊（這裡先用測試資料，之後會從預約存檔中取得）
        nameText.text = activityForTest.activityName;
        descriptionText.text = activityForTest.description;
        VigourCostText.text = $"{activityForTest.vigourCost} 體";
        // MoneyGainText.text = $"{} 錢";

        // 設定角色 UI 圖片及位置（不必隨時存檔，派出去再處理就好）
        // 其他初始化

    }

    public void CloseActivityUI()
    {
        Debug.Log("關閉外出商演 UI");
        activityUI.SetActive(false);
        UIAndPlayerInput.EnableAllPlayerInputs(); // 允許角色走動
    }
   
}