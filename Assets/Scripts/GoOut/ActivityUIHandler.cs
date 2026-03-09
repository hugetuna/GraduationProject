using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField] private List<Image> characterImages; // 角色圖片槽 
    [SerializeField] private Button confirmBtn; // 確認出發的按鈕
    //-----------------------------------------------------------------//
    [Header("隊伍資訊")]
    [SerializeField] private TeamManager teamManager; 
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟介面的音效
    //-----------------------------------------------------------------//
    [Header("商演資訊")]
    [SerializeField] private Activity activityForTest; // 商演資料 SO（測試用，之後會從預約存檔中取得）
    //-----------------------------------------------------------------//
    // private bool isInitialized = false;

    void Start()
    {
        GoOutInteraction.OnExitInteracted += ShowActivityUI; // 訂閱出門事件
        closeButton.onClick.AddListener(CloseActivityUI); // 設定關閉按鈕的監聽事件
        confirmBtn.onClick.AddListener(ConfirmToActivity); // 設定出發按鈕的監聽事件
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
        MoneyGainText.text = $"{activityForTest.MoneyGain} 錢";

        // 設定角色 UI 圖片及位置（不必隨時存檔，派出去再處理就好）
        UpdateCharacterImagesAndPositions();

        // 其他初始化（僅限在場景中第一次打開 UI）
        // if (!isInitialized)
        // {
        //     DragToActivity[] dragToActivity = activityUI.GetComponentsInChildren<DragToActivity>();
        //     foreach (DragToActivity dtl in dragToActivity)
        //     {
        //         dtl.Initialize(); // 初始化底下每一個 DragToActivity 元件
        //     }

        //     numsController.InitializeSlots(); // 初始化數值顯示

        //     isInitialized = true;
        // }
        // else
        // {
        //     numsController.RefreshSlots(); // 刷新角色數值顯示
        // }
    }
    
    public void CloseActivityUI()
    {
        Debug.Log("關閉外出商演 UI");
        activityUI.SetActive(false);
        UIAndPlayerInput.EnableAllPlayerInputs(); // 允許角色走動
    }

    private void UpdateCharacterImagesAndPositions()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];

            // 為圖片插槽放置角色圖片
            if (i < TeamDataUtility.idolCount)
            {
                img.sprite = TeamDataUtility.QSprites.ElementAt(i).Value;
            }
            else
            {
                // 超出角色範圍就設為空，避免錯誤
                img.sprite = null;
                continue;
            }

            // 判斷該角色是否在隊伍裡（或是已經被指派去做其他事）
            var idol = TeamDataUtility.IdolObjectList[i].GetComponent<PlayerControlMainWorld>();
            bool isInTeam = !teamManager.busyMembers.Contains(idol);
            img.gameObject.SetActive(isInTeam);

            // 每次打開 UI 都會刷新角色位置，不用特別還原
        }
    }

    private void ConfirmToActivity()
    {
        // 指派商演
    }
}