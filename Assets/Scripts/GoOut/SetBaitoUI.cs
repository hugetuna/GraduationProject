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
    private List<Vector2> originalPositions = new(); // 角色圖片的初始位置列表
    [SerializeField] private List<BaitoDropZone> characterDropZones = new(); // 角色初始放置的 DropZone
    //-----------------------------------------------------------------//
    [SerializeField] private Button confirmBtn; // 確認出發的按鈕
    public static event Action OnBaitoConfirmed; // 定義確認出發事件
    //-----------------------------------------------------------------//
    [Header("打工資訊")]
    [SerializeField] private List<Baito> baitoList = new(); // 可選的打工列表
    private int currentBaitoIndex = 0; // 目前選擇的打工索引
    private bool isInitialized = false;
    public static event Action<Baito> OnBaitoChanged; // 定義變更打工選擇事件

    void Start()
    {
        InitializeButtonEvents(); // 初始化按鈕事件
    }

    public void OpenBaitoUI() // 每次開啟介面時都會執行一次
    {
        // 預設顯示第一個打工選項的資訊
        currentBaitoIndex = 0;
        UpdateBaitoInfo(baitoList[currentBaitoIndex]);

        if (!isInitialized)
        {
            foreach (var img in characterImages) // 儲存角色圖片的初始位置
            {
                originalPositions.Add(img.rectTransform.anchoredPosition);
            }

            isInitialized = true;
        }

        UpdateCharacterImagesAndPositions(); // 設定角色 UI 圖片及位置（不必隨時存檔，派出去再處理就好）

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
        OnBaitoConfirmed?.Invoke(); // 觸發確認出發事件，指派角色外出打工
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
            bool isInTeam = TeamDataUtility.IdolObjectList[i].activeSelf;
            img.gameObject.SetActive(isInTeam);

            // 重置圖片位置
            img.rectTransform.anchoredPosition = originalPositions[i];
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

            var idol = idolInstanceList[i].idolIndex;
            var data = baitoList[currentBaitoIndex];
            var characterDropZone = characterDropZones[i];

            drag.Initialize(idol, characterDropZone); // 初始化角色底下的拖曳元件
            characterDropZone.SetCurrentIdol(drag); // 登記角色的初始 DropZone 位置
            vigourBar.Initialize(data, idol); // 初始化角色底下的體力條
            numbers.Initialize(idol); // 初始化角色底下的數值顯示
        }
    }
}