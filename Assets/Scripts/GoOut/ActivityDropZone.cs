using UnityEngine;
using UnityEngine.EventSystems;

public enum ActivityDropZoneType { None = -1, Member = 0, Activity = 1 }
public class ActivityDropZone : Drop
{
    public ActivityDropZoneType zoneType; // 直接在 Inspector 設定即可
    public int zoneIndex; // 同一類型的區域可能有多個，從 0 開始編號
    private DragToActivity currentIdol; // 目前待在這格的角色


    // void Awake(); // 使用父類別的預設內容

    // 進入區域時，通知拖曳物件
    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 基本檢查
        if (eventData == null) return;

        var draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // 重點設定
        var drag = draggedObject.GetComponent<DragToActivity>();
        if (drag != null)
        {
            if (currentIdol != null && currentIdol != drag) return;
            drag.CurrentDropZone = this;
        }
    }

    // 離開區域時，清空拖曳物件的參考
    public override void OnPointerExit(PointerEventData eventData)
    {
        // 基本檢查
        if (eventData == null) return;

        var draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // 重點設定
        var drag = draggedObject.GetComponent<DragToActivity>();
        if (drag != null && drag.CurrentDropZone == this)
        {
            drag.CurrentDropZone = null;
        }
    }
    
    public void SetCurrentIdol(DragToActivity draggedIdol)
    {
        currentIdol = draggedIdol;
    }

    public void ClearCurrentIdol() => currentIdol = null;
}
