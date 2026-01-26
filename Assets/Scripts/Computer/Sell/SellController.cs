using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在販賣頁面根部 */
public class SellController : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Button transformButton; // 轉換按鈕
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject moneyPage; // 金錢頁面
    private TextMeshProUGUI moneyText;
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject fansPage; // 粉絲頁面
    private TextMeshProUGUI fansText;
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject itemPage; // 道具頁面
    [SerializeField] private Transform dropItemParent; // 掉落道具父物件
    private List<SetFansDropItemUI> fixeditemSlots = new(); // 掉落道具父物件底下的道具物件
    [SerializeField] private Button lastButton; // 上一頁按鈕
    [SerializeField] private Button nextButton; // 下一頁按鈕
    [SerializeField] private int itemPerPage = 5; // 一頁顯示幾個
    private List<ItemStack> allItemData = new(); // 儲存所有道具資料
    private int currentPage = 0;
    //-----------------------------------------------------------------//
    private List<GameObject> fansToSellList = new();
    private int totalMoney = 0;
    private int totalFans = 0;
    private Dictionary<Item, int> totalItems = new();
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip sellSound; // 售出音效

    void Start()
    {
        transformButton.onClick.AddListener(TransformFansToSth);
        transformButton.interactable = false; // 轉換按鈕預設為不可點擊

        // 初始化各個頁面
        moneyText = moneyPage.GetComponentInChildren<TextMeshProUGUI>();
        moneyText.text = "= $0";
        fansText = fansPage.GetComponentInChildren<TextMeshProUGUI>();
        fansText.text = "= 0 個粉絲數";

        // 取得所有掉落道具 UI
        var slots = dropItemParent.GetComponentsInChildren<SetFansDropItemUI>(true);
        fixeditemSlots.AddRange(slots);
    }

    public void AddToFansUIList(GameObject fansUI)
    {
        if (!fansToSellList.Contains(fansUI))
        {
            fansToSellList.Add(fansUI);
            UpdateSellMenuUI();
        }
    }

    public void RemoveFromFansUIList(GameObject fansUI)
    {
        if (fansToSellList.Contains(fansUI))
        {
            fansToSellList.Remove(fansUI);
            UpdateSellMenuUI();
        }
    }

    private void UpdateSellMenuUI()
    {
        // 根據是否有粉絲可賣，設定轉換按鈕的互動性
        if (fansToSellList.Count > 0) transformButton.interactable = true;
        else transformButton.interactable = false;

        // 一次更新全部的 UI
        UpdateMoneyPage();
        UpdateFansPage();
        UpdateItemPage();
    }

    private void UpdateMoneyPage()
    {
        totalMoney = 0;
        foreach (var fansUI in fansToSellList)
        {
            var fansObjectUI = fansUI.GetComponent<SetFansObjectUI>();
            if (fansObjectUI != null)
            {
                var fansItemStack = fansObjectUI.FansItemStack;
                FansItem fansItem = fansItemStack.item as FansItem;
                totalMoney += fansItem.moneyPower * fansItemStack.quantity;
            }
        }
        moneyText.text = $"= ${totalMoney:N0}";
    }

    private void UpdateFansPage()
    {
        totalFans = 0;
        foreach (var fansUI in fansToSellList)
        {
            var fansObjectUI = fansUI.GetComponent<SetFansObjectUI>();
            if (fansObjectUI != null)
            {
                var fansItemStack = fansObjectUI.FansItemStack;
                FansItem fansItem = fansItemStack.item as FansItem;
                totalFans += fansItem.OShiPower * fansItemStack.quantity;
            }
        }
        fansText.text = $"= {totalFans:N0} 個粉絲數";
    }

    private void UpdateItemPage()
    {
        totalItems.Clear();
        foreach (var fansUI in fansToSellList)
        {
            var fansObjectUI = fansUI.GetComponent<SetFansObjectUI>();
            if (fansObjectUI != null)
            {
                var fansItemStack = fansObjectUI.FansItemStack;
                FansItem fansItem = fansItemStack.item as FansItem;
                
                foreach (var dropItem in fansItem.dropableItems)
                {
                    if (totalItems.ContainsKey(dropItem))
                    {
                        totalItems[dropItem] += fansItemStack.quantity;
                    }
                    else
                    {
                        totalItems[dropItem] = fansItemStack.quantity;
                    }
                }
            }
        }
        HandleItemPool();
        UpdatePageToggler();
    }

    private void HandleItemPool()
    {
        allItemData.Clear();
        foreach (var item in totalItems)
        {
            allItemData.Add(new ItemStack(item.Key, item.Value));
        }

        if (fixeditemSlots.Count == 0)
        {
            Debug.LogError($"掉落道具刷新失敗！");
            return;
        }

        int startIndex = currentPage * itemPerPage;

        // 直接跑固定數量的格子
        for (int i = 0; i < fixeditemSlots.Count; i++)
        {
            int dataIndex = startIndex + i;
            SetFansDropItemUI slot = fixeditemSlots[i];

            if (dataIndex < allItemData.Count)
            {
                // 有資料，顯示並更新
                slot.gameObject.SetActive(true);
                slot.Initialize(allItemData[dataIndex]);
            }
            else
            {
                // 沒資料，把多餘的格子藏起來
                slot.gameObject.SetActive(false);
            }
        }
    }

    private void UpdatePageToggler()
    {
        int totalPages = Mathf.Max(1, Mathf.CeilToInt((float)totalItems.Count / itemPerPage));

        // 第一頁不能按上一頁，最後一頁不能按下一頁
        lastButton.interactable = currentPage > 0;
        nextButton.interactable = (currentPage + 1) < totalPages;
    }

    public void NextPage()
    {
        currentPage++;
        UpdateItemPage();
    }

    public void PrevPage()
    {
        currentPage--;
        UpdateItemPage();
    }

    private void TransformFansToSth()
    {
        if (moneyPage.activeSelf)
        {
            ResourceManager.Instance.GainMoney(totalMoney);
            totalMoney = 0;
        }
        else if (fansPage.activeSelf)
        {
            // 根據粉絲道具的擁有者，增加該角色的粉絲數
            // 因為目前只寫了一個角色，暫時先硬寫
            TeamDataUtility.IdolInstanceList[0].fans += totalFans;
            totalFans = 0;
        }
        else if (itemPage.activeSelf)
        {
            foreach (var item in totalItems)
            {
                ResourceManager.Instance.AddItem(item.Key, item.Value);
            }
            totalItems.Clear();
        }

        // 移除已售出的粉絲（FansItem）
        foreach (var fansUI in fansToSellList)
        {
            var fansObjectUI = fansUI.GetComponent<SetFansObjectUI>();
            if (fansObjectUI != null)
            {
                var fansItemStack = fansObjectUI.FansItemStack;
                ResourceManager.Instance.RemoveItem(fansItemStack.item, fansItemStack.quantity);
            }
        }

        // 清空待售清單
        foreach (var fansUI in fansToSellList)
        {
            Destroy(fansUI);
        }
        fansToSellList.Clear();

        // 更新 UI
        UpdateSellMenuUI();

        // 播放售出音效
        if(sellSound != null)
        {
            AudioManager.Instance.PlaySFX(sellSound);
        }
    }
}
