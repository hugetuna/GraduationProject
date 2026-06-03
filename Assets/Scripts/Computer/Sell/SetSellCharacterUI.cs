using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在販賣頁面的各個角色根部 */
public class SetSellCharacterUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Image headImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button lastButton; // 上一頁按鈕
    [SerializeField] private Button nextButton; // 下一頁按鈕
    [SerializeField] private TextMeshProUGUI pageText; // 頁數顯示
    //-----------------------------------------------------------------//
    [SerializeField] private Transform fansObjectParent; // 粉絲父物件
    private List<SetFansObjectUI> fixedFansSlots = new(); // 粉絲父物件底下的粉絲物件
    [SerializeField] private int fansPerPage = 10; // 一頁顯示幾個
    private List<ItemStack> allFansData = new(); // 儲存所有粉絲資料
    private int currentPage = 0;
    private IdolInstance idolInstance;

    void Start()
    {
        SellController.OnSellConfirmedWithFans += HandleSoldFans; // 訂閱售出確認事件

        // 綁定按鈕事件
        lastButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);
    }

    void OnDestroy()
    {
        SellController.OnSellConfirmedWithFans -= HandleSoldFans; // 取消訂閱事件
    }

    public void Initialize(IdolInstance idol, List<ItemStack> fansList)
    {
        idolInstance = idol;

        // 取得該角色所有粉絲物件 UI
        fixedFansSlots.Clear(); // 以防萬一
        var slots = fansObjectParent.GetComponentsInChildren<SetFansObjectUI>(true);
        fixedFansSlots.AddRange(slots);

        // 設定角色 UI
        headImage.sprite = idolInstance.idolUISprites.spriteTachie;
        nameText.text = TeamDataUtility.GetIdolNameTW(idolInstance.idolIndex);

        // 儲存粉絲資料並重設其 UI
        allFansData = fansList ?? new List<ItemStack>(); // 若清單為 null，則給予空清單
        currentPage = 0;
        RefreshFansUI();
    }

    private void RefreshFansUI()
    {
        if (fixedFansSlots.Count == 0)
        {
            Debug.LogError($"粉絲資訊刷新失敗！");
            return;
        }

        int startIndex = currentPage * fansPerPage;

        // 直接跑固定數量的格子
        for (int i = 0; i < fixedFansSlots.Count; i++)
        {
            int dataIndex = startIndex + i;
            SetFansObjectUI slot = fixedFansSlots[i];

            if (dataIndex < allFansData.Count)
            {
                // 有資料，顯示並更新
                slot.gameObject.SetActive(true);
                slot.Initialize(allFansData[dataIndex], idolInstance.idolIndex);
                // Debug.Log($"顯示粉絲資料{dataIndex}：{allFansData[dataIndex].item.itemName}");
            }
            else
            {
                // 沒資料，把多餘的格子藏起來
                slot.gameObject.SetActive(false);
            }
        }

        UpdatePageToggler();
    }

    private void UpdatePageToggler()
    {
        int totalPages = Mathf.Max(1, Mathf.CeilToInt((float)allFansData.Count / fansPerPage));

        // 更新頁碼文字 (格式為 1/1)
        pageText.text = $"{currentPage + 1}/{totalPages}";

        // 第一頁不能按上一頁，最後一頁不能按下一頁
        lastButton.interactable = currentPage > 0;
        nextButton.interactable = (currentPage + 1) < totalPages;
    }

    public void NextPage()
    {
        currentPage++;
        RefreshFansUI();
    }

    public void PrevPage()
    {
        currentPage--;
        RefreshFansUI();
    }

    public void HandleSoldFans(List<ItemStack> soldFans)
    {
        // 移除已售出的粉絲
        foreach (var sold in soldFans)
        {
            // 1. 用 FindIndex 抓到該道具在清單中的「真實陣列索引」
            int index = allFansData.FindIndex(f => f.item != null && f.item.itemID == sold.item.itemID);

            // 如果有找到（index 不等於 -1）
            if (index != -1)
            {
                // 2. 拿出本體資料
                ItemStack stack = allFansData[index];
                stack.quantity -= sold.quantity;

                if (stack.quantity <= 0)
                {
                    // 3. 數量小於等於 0，直接移除
                    allFansData.RemoveAt(index);
                }
                else
                {
                    // 4. 如果只是扣數量，必須把修改後的 struct 重新塞回清單對應位置
                    allFansData[index] = stack;
                }
            }
        }
        RefreshFansUI();
    }
}
