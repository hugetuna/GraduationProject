using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragger : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform windowRect;
    private Vector2 pointerOffset;

    void Awake()
    {
        // 腳本預計掛在標題列的位置（父物件是視窗本人）
        windowRect = transform.parent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 一開始拖曳就把視窗置頂
        transform.parent.SetAsLastSibling();

        // 記錄滑鼠點擊位置與視窗左下角的距離
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowRect,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 更新視窗位置
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            windowRect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            windowRect.localPosition = localPoint - pointerOffset;
        }
    }
}
