using UnityEngine.EventSystems;
using UnityEngine;

// public enum FansDropZoneType
// {
//     // 一種粉絲只能拖曳到擁有者或販賣區域
//     None,
//     MemberA,
//     MemberB,
//     MemberC,
//     Sell
// }

/* 掛在販賣頁面的可拖曳區域上 */
public class FansDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // public FansDropZoneType zoneType; // 直接在 Inspector 設定即可
    // public int zoneIndex; // 同一類型的區域可能有多個，從 0 開始編號
    //-----------------------------------------------------------------//
    private RectTransform myRect;
    public RectTransform MyRect => myRect;

    void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    // 進入區域時，通知拖曳物件
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 基本檢查
        if (eventData == null) return;

        var draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // 重點設定
        var drag = draggedObject.GetComponent<DragToSell>();
        if (drag != null)
        {
            drag.CurrentDropZone = this;
        }
    }

    // 離開區域時，清空拖曳物件的參考
    public void OnPointerExit(PointerEventData eventData)
    {
        // 基本檢查
        if (eventData == null) return;

        var draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // 重點設定
        var drag = draggedObject.GetComponent<DragToLesson>();
        if (drag != null && drag.CurrentDropZone == this)
        {
            drag.CurrentDropZone = null;
        }
    }
}
