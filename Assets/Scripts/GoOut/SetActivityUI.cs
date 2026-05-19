using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;

/* 掛在商演介面上 */
public class SetActivityUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Button closeButton; // 關閉介面的按鈕
    [SerializeField] private TextMeshProUGUI activityText; // 商演名稱文字
    [SerializeField] private TextMeshProUGUI descriptionText; // 商演說明文字
    [SerializeField] private List<Image> characterImages = new(); // 角色圖片槽
    [SerializeField] private List<Image> characterEquipments = new(); // 角色裝備圖片槽
    [SerializeField] private TextMeshProUGUI VigourCostText; // 商演耗體文字
    [SerializeField] private TextMeshProUGUI MoneyGainText; // 商演收益文字
    //-----------------------------------------------------------------//
    [SerializeField] private Button confirmBtn; // 確認出發的按鈕
    public static event Action<Activity, StageAttribute> OnActivityConfirmed; // 定義確認出發事件
    //-----------------------------------------------------------------//

    [Header("商演資料")]
    private Activity todayActivity = null;
    [SerializeField] private StageAttribute StageAttributeForTest; // 對應的舞台資料
    // private bool isInitialized = false;
    //-----------------------------------------------------------------//
    [Header("音效設定")]
    [SerializeField] private AudioClip cancelSound;

    void Start()
    {
        InitializeButtonEvents(); // 初始化按鈕事件
    }

    public void OpenActivityUI(Activity appointedActivity) // 每次開啟介面時都會執行一次
    {
        // 根據預約紀錄顯示商演資訊
        todayActivity = appointedActivity;
        activityText.text = todayActivity.activityName;
        descriptionText.text = todayActivity.description;
        VigourCostText.text = $"{todayActivity.vigourCost} 體";
        MoneyGainText.text = $"{todayActivity.MoneyGain} 錢";

        UpdateCharacterImagesAndEquipments(); // 設定角色 UI 與裝備欄圖片
        RefreshCharacterStats(); // 刷新體力狀態與角色數值
    }

    private void InitializeButtonEvents()
    {
        closeButton.onClick.AddListener(CloseActivityUI); // 為關閉按鈕添加點擊事件
        confirmBtn.onClick.AddListener(ConfirmToActivity); // 為確認出發按鈕添加點擊事件
    }

    private void CloseActivityUI()
    {
        AudioManager.Instance.PlaySFX(cancelSound);
        GoOutUIHandler.TriggerUIsClosedEvent(); // 觸發事件，返回選擇介面
        gameObject.SetActive(false);
    }

    private void ConfirmToActivity()
    {
        Debug.Log("指派外出商演");
        OnActivityConfirmed?.Invoke(todayActivity, StageAttributeForTest); // 觸發確認出發事件，指派全員外出商演
    }

    private void UpdateCharacterImagesAndEquipments()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            var idol = TeamDataUtility.IdolInstanceList[i];
            Image img = characterImages[i];
            Image equip = characterEquipments[i];

            // 為圖片插槽放置角色圖片
            if (i < TeamDataUtility.idolCount)
            {
                img.sprite = idol.idolUISprites.spriteQ;
            }
            else
            {
                // 超出角色範圍就設為空，避免錯誤
                img.sprite = null;
                continue;
            }

            // 判斷該角色是否在隊伍裡（已經被指派去做其他事）
            // => 全員都會顯示，不在隊伍的人會變半透明 
            // => 若要去商演，不論角色在做什麼都會強制召回
            // 要用 idol.CanShowInTheAction(AvailableAction.Activity) 來判斷也行，但這裡既然沒問題就不想改了
            bool isAvailable = idol.isAvailable;
            img.color = isAvailable ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);

            // 圖片位置不會動所以不用設定

            // 設定裝備圖片（因為不會太複雜所以寫在一起）
            if (idol.equipmentItemNow != null)
            {
                equip.sprite = idol.equipmentItemNow.icon;
            }
            else equip.sprite = null;
        }
    }

    private void RefreshCharacterStats()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            var idol = TeamDataUtility.IdolInstanceList[i];

            var vigourBar = characterImages[i].GetComponentInChildren<ActivityVigourBar>();
            var numbers = characterImages[i].GetComponentInChildren<GoOutNumbers>();

            vigourBar.Initialize(todayActivity, idol); // 初始化體力條
            numbers.Initialize(idol.idolIndex); // 初始化角色底下的數值顯示
        }
    }
}
