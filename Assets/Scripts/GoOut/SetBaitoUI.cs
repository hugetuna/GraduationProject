using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

/* 掛在打工介面上 */
public class SetBaitoUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Button closeButton; // 關閉介面的按鈕
    //-----------------------------------------------------------------//
    [SerializeField] private TextMeshProUGUI baitoText; // 打工類型文字
    [SerializeField] private Button lastButton; // 上一個打工選項的按鈕
    [SerializeField] private Button nextButton; // 下一個打工選項的按鈕
    [SerializeField] private TextMeshProUGUI descriptionText; // 打工說明文字
    [SerializeField] private TextMeshProUGUI VigourCostText; // 打工耗體文字
    [SerializeField] private TextMeshProUGUI MoneyGainText; // 打工收益文字
    //-----------------------------------------------------------------//
    [SerializeField] private List<Image> characterImages = new(); // 角色圖片槽
    [SerializeField]private List<BaitoDropZone> memberDropZones = new();
    [SerializeField]private List<BaitoDropZone> baitoDropZones = new();
    //-----------------------------------------------------------------//
    [SerializeField] private Button confirmBtn; // 確認出發的按鈕
    public static event Action<Baito> OnBaitoConfirmed; // 定義確認出發事件
    //-----------------------------------------------------------------//
    [Header("打工資訊")]
    [SerializeField] private List<Baito> baitoList = new(); // 可選的打工列表
    private int currentBaitoIndex = 0; // 目前選擇的打工索引
    public static event Action<Baito> OnBaitoChanged; // 定義變更打工選擇事件

    void Start()
    {
        InitializeButtonEvents(); // 初始化按鈕事件
    }

    public void OpenBaitoUI() // 每次開啟介面時都會執行一次
    {
        // 顯示目前選擇的打工資訊（換場景即重置）
        UpdateBaitoInfo(baitoList[currentBaitoIndex]);

        UpdateCharacterImagesAndPositions(); // 設定角色 UI 圖片及位置

        RefreshDragSystem(); // 刷新拖曳系統
    }

    private void UpdateBaitoInfo(Baito baitoData)
    {
        baitoText.text = baitoData.baitoName;
        descriptionText.text = baitoData.description;
        VigourCostText.text = $"{baitoData.vigourCost} 體";
        MoneyGainText.text = $"{baitoData.MoneyGain} 錢";
    }

    private void InitializeButtonEvents()
    {
        closeButton.onClick.AddListener(CloseBaitoUI); // 為關閉按鈕添加點擊事件
        lastButton.onClick.AddListener(ChooseLastBaito); // 為上一個選項按鈕添加點擊事件
        nextButton.onClick.AddListener(ChooseNextBaito); // 為下一個選項按鈕添加點擊事件
        confirmBtn.onClick.AddListener(ConfirmToBaito); // 為確認出發按鈕添加點擊事件
    }

    private void CloseBaitoUI()
    {
        gameObject.SetActive(false);
    }

    private void ChooseLastBaito()
    {
        // 根據打工清單順序，切換成上一個選項（記得不要超出範圍）
        currentBaitoIndex = (currentBaitoIndex - 1 + baitoList.Count) % baitoList.Count;
        UpdateBaitoInfo(baitoList[currentBaitoIndex]);
        OnBaitoChanged?.Invoke(baitoList[currentBaitoIndex]);
    }

    private void ChooseNextBaito()
    {
        // 根據打工清單順序，切換成下一個選項（記得不要超出範圍）
        currentBaitoIndex = (currentBaitoIndex + 1) % baitoList.Count;
        UpdateBaitoInfo(baitoList[currentBaitoIndex]);
        OnBaitoChanged?.Invoke(baitoList[currentBaitoIndex]);
    }

    private void ConfirmToBaito()
    {
        Debug.Log("指派外出打工");
        OnBaitoConfirmed?.Invoke(baitoList[currentBaitoIndex]); // 觸發確認出發事件，指派角色外出打工
    }

    private void UpdateCharacterImagesAndPositions()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];
            IdolInstance idol = TeamDataUtility.IdolInstanceList[i];

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

            // 還原上次圖片位置，如果沒有初始化
            if(idol.baitoRecord.position != Vector2.zero)
            {
                img.rectTransform.anchoredPosition = idol.baitoRecord.position;
            }
            else
            {
                idol.baitoRecord.position = img.rectTransform.anchoredPosition;
            }
        }
    }

    private void RefreshDragSystem()
    {
        var idolInstanceList = TeamDataUtility.IdolInstanceList;
        for (int i = 0; i < characterImages.Count; i++)
        {
            var drag = characterImages[i].GetComponentInChildren<DragToBaito>();
            var vigourBar = characterImages[i].GetComponentInChildren<BaitoVigourBar>();
            var numbers = characterImages[i].GetComponentInChildren<GoOutNumbers>();
            var assignEffect = characterImages[i].GetComponentInChildren<BaitoAssignEffect>();

            var idol = idolInstanceList[i];
            var idolIndex = idol.idolIndex;
            var data = baitoList[currentBaitoIndex];
            
            if(idol.baitoRecord.zoneIndex == -1) // 只要有一人是 -1，就代表全員尚未初始化
            {
                idol.baitoRecord.zoneIndex = i; // 預設分配到對應的圖片位置
                // 其他預設值就不特別碰了
            }
            
            // 刷新前先清空上次位置
            memberDropZones.ForEach(zone => zone.ClearCurrentIdol()); 
            baitoDropZones.ForEach(zone => zone.ClearCurrentIdol());
            BaitoDropZone characterDropZone; // 正式還原＆分配位置
            if(idol.baitoRecord.zoneType == BaitoDropZoneType.Baito) 
            {
                characterDropZone = baitoDropZones.FirstOrDefault(zone => zone.zoneIndex == idol.baitoRecord.zoneIndex);
            }
            else
            {
                characterDropZone = memberDropZones.FirstOrDefault(zone => zone.zoneIndex == idol.baitoRecord.zoneIndex);
            }

            // 初始化拖曳元件、體力條、數值顯示並登記 DropZone 位置
            drag.Initialize(idolIndex, characterDropZone);
            characterDropZone.SetCurrentIdol(drag);
            vigourBar.Initialize(data, idolIndex);
            numbers.Initialize(idolIndex);
            assignEffect.Initialize(idol.baitoRecord.selectedBaito);
        }
    }
}