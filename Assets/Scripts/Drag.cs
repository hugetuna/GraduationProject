using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

/* 拖曳功能共用的父類別 */
public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected RectTransform rectTransform;
    protected Canvas canvas;
    protected CanvasGroup canvasGroup;
    //-----------------------------------------------------------------//
    [Header("拖曳後的偏移")]
    [SerializeField] protected Vector3 dropOffset = new(0f, 0f, 0f);
    protected Vector2 originalPosition;
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] protected AudioClip dragCompletedSound; // 拖曳成功的音效

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    
        if(GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup = GetComponent<CanvasGroup>();

        originalPosition = rectTransform.anchoredPosition;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // isDragging = true;
        canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
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

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // isDragging = false;
        canvasGroup.blocksRaycasts = true;
    }
}
