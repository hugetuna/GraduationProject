using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

/* 掛在販賣頁面的粉絲 prefab 上 */
public class DragToSell : Drag
{
    [Header("位置資料")]
    // private bool isDragging = false;
    [SerializeField] private FansDropZoneType ownerType; // 該粉絲的擁有者
    public FansDropZoneType OwnerType => ownerType;
    //-----------------------------------------------------------------//
    private FansDropZone lastDropZone = null; // 紀錄上一次成功放置的 DropZone
    private FansDropZone currentDropZone = null; // 當前放置的 DropZone
    public FansDropZone CurrentDropZone
    {
        get { return currentDropZone; }
        set { currentDropZone = value; }
    }
    private SellController sellController;
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private GameObject fansNameText;
    [SerializeField] private GameObject fansOwnerIcon;

    protected override void Awake()
    {
        base.Awake();

        // 粉絲物件的 prefab 都放在 Sell UI 底下
        sellController = GetComponentInParent<SellController>();
    }

    void Start()
    {
        SellController.OnSellConfirmed += ForceReturnToOwnerZone;
    }

    void OnDestroy()
    {
        SellController.OnSellConfirmed -= ForceReturnToOwnerZone;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        fansNameText.SetActive(false);
        fansOwnerIcon.SetActive(false);
    }

    // public void OnDrag(PointerEventData eventData); // 使用父類別的預設內容

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        bool isSellZone = currentDropZone != null && currentDropZone.zoneType == FansDropZoneType.Sell;
        bool isOwnerZone = currentDropZone != null && currentDropZone.zoneType == OwnerType;

        if (isSellZone)
        {
            // 成功拖曳到販賣區域 (不吸附，停留在放開的位置) 
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f);

            // 不需要設定 rectTransform.position，因為 OnDrag 已經把它帶到這了

            lastDropZone = currentDropZone; // 更新位置紀錄

            Debug.Log("已將該粉絲成功拖曳到販賣區域");
        }
        else if (isOwnerZone)
        {
            // 成功拖曳到擁有者的小格子 (執行吸附) 
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f);

            // 強制吸附到格子的中心點
            rectTransform.position = currentDropZone.MyRect.position + dropOffset;

            lastDropZone = currentDropZone; // 更新位置紀錄
        }
        else if (lastDropZone != null)
        {
            // 拖曳失敗，回到上一個成功的位置
            rectTransform.position = lastDropZone.MyRect.position + dropOffset;
        }
        else
        {
            // 拖曳失敗，沒有上個 DropZone，回到原始位置
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
        }

        // 統一狀態管理
        bool isInSellZone = lastDropZone != null && lastDropZone.zoneType == FansDropZoneType.Sell;
        UpdateSellStatus(isInSellZone);
    }
    
    private void UpdateSellStatus(bool isInSellZone)
    {
        // 處理欲販賣清單
        if (isInSellZone) sellController.AddToFansUIList(gameObject);
        else sellController.RemoveFromFansUIList(gameObject);

        // 處理 UI 樣式
        fansNameText.SetActive(!isInSellZone);
        fansOwnerIcon.SetActive(isInSellZone);
    }

    public void ForceReturnToOwnerZone() // 販賣粉絲後強制回到最原始的位置
    {
        rectTransform.anchoredPosition = originalPosition;
        UpdateSellStatus(false); // 回到擁有者區域＆更新狀態
        lastDropZone = null;
        currentDropZone = null;
    }
}
