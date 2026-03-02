using UnityEngine;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

/* 拖曳區域共用的父類別 */
public abstract class Drop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected RectTransform myRect;
    public RectTransform MyRect => myRect;

    protected virtual void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    // 進入區域時，通知拖曳物件
    public abstract void OnPointerEnter(PointerEventData eventData);

    // 離開區域時，清空拖曳物件的參考
    public abstract void OnPointerExit(PointerEventData eventData);
}
