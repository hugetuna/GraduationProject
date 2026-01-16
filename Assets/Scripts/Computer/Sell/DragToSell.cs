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
    // private bool isDragging = false;
    private FansDropZone lastDropZone = null; // 紀錄上一次成功放置的 DropZone
    private FansDropZone currentDropZone = null; // 當前放置的 DropZone
    public FansDropZone CurrentDropZone
    {
        get { return currentDropZone; }
        set { currentDropZone = value; }
    }
    //-----------------------------------------------------------------//
    // [Header("拖曳時受影響的 UI 元素")]
    // [SerializeField] private GameObject vigourSlider;
    // private VigourBar vigourBar; // 對應腳本參考
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip dragCompletedSound; // 拖曳成功的音效

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

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

        // 拖曳成功，放到新的 DropZone
        if (currentDropZone != null) // 更新最後成功 DropZone
        {
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f); // 播放拖曳成功音效
            rectTransform.position = currentDropZone.MyRect.position + dropOffset;

            lastDropZone = currentDropZone;
        }
        else if (lastDropZone != null) // 拖曳失敗，回到上個 DropZone
        {
            rectTransform.position = lastDropZone.MyRect.position + dropOffset;
        }
        else // 拖曳失敗，沒有上個 DropZone，回到原始位置
        {
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
        }
    }
}
