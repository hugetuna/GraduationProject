using UnityEngine;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

/* 掛在販賣頁面的粉絲 prefab 上 */
public class DragToSell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    //-----------------------------------------------------------------//
    [Header("拖曳後的偏移")]
    [SerializeField] private Vector3 dropOffset = new(0f, 0f, 0f);
    private Vector2 originalPosition;
    //-----------------------------------------------------------------//
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
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip dragCompletedSound; // 拖曳成功的音效

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // 粉絲物件的 prefab 都放在 Sell UI 底下
        sellController = GetComponentInParent<SellController>();

        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup.interactable == false)
        {
            Debug.Log("無法拖曳");
            return;
        }

        // isDragging = true;
        canvasGroup.blocksRaycasts = false;

        fansNameText.SetActive(false);
        fansOwnerIcon.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 將滑鼠位置轉成世界座標，直接設置物件位置
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 globalMousePos))
        {
            rectTransform.position = globalMousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // isDragging = false;
        canvasGroup.blocksRaycasts = true;

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
}
