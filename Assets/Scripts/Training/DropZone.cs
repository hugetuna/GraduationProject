using UnityEngine;
using UnityEngine.EventSystems;

public enum DropZoneType
{
    None = -1,
    Member = 0, // 隊伍區域
    Dance = 1,  // 舞蹈訓練室
    Vocal = 2,  // 歌唱訓練室
    Visual = 3  // 表現力訓練室
}

public class DropZone : Drop
{
    public DropZoneType zoneType; // 直接在 Inspector 設定即可
    public int zoneIndex; // 同一類型的區域可能有多個，從 0 開始編號


    // void Awake(); // 使用父類別的預設內容

    // 進入區域時，通知拖曳物件
    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 基本檢查
        if (eventData == null) return;

        var draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // 重點設定
        var drag = draggedObject.GetComponent<DragToLesson>();
        if (drag != null)
        {
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
        var drag = draggedObject.GetComponent<DragToLesson>();
        if (drag != null && drag.CurrentDropZone == this)
        {
            drag.CurrentDropZone = null;
        }
    }
}

