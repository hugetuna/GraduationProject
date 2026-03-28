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
    //-----------------------------------------------------------------//
    [SerializeField] private Button confirmBtn; // 確認出發的按鈕
    public static event Action OnActivityConfirmed; // 定義確認出發事件
    //-----------------------------------------------------------------//

    [Header("商演資料（測試用）")]
    [SerializeField] private Activity ActivityForTest;
    // private bool isInitialized = false;

    void Start()
    {
        InitializeButtonEvents(); // 初始化按鈕事件
    }

    public void OpenActivityUI() // 每次開啟介面時都會執行一次
    {
        // 根據預約紀錄顯示商演資訊（暫時先用測試資料）
        activityText.text = ActivityForTest.activityName;
        descriptionText.text = ActivityForTest.description;

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
        gameObject.SetActive(false);
        GoOutUIHandler.TriggerUIsClosedEvent(); // 觸發事件，返回選擇介面
    }

    private void ConfirmToActivity()
    {
        OnActivityConfirmed?.Invoke(); // 觸發確認出發事件，指派全員外出商演
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
                img.sprite = TeamDataUtility.QSprites[idol.idolIndex];
            }
            else
            {
                // 超出角色範圍就設為空，避免錯誤
                img.sprite = null;
                continue;
            }

            // 判斷該角色是否在隊伍裡（或是已經被指派去做其他事）=> 若沒有全員到齊就不得出發商演
            bool isInTeam = idol.gameObject.activeSelf;
            img.gameObject.SetActive(isInTeam);

            // 圖片位置不會動所以不用設定

            // 設定裝備圖片（因為不會太複雜所以寫在一起）
            if (isInTeam && idol.equipmentItemNow != null)
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

            vigourBar.Initialize(ActivityForTest, idol); // 初始化體力條
            numbers.Initialize(idol.idolIndex); // 初始化角色底下的數值顯示
        }
    }
}
