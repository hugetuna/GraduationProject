using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

public class DragToLesson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;
    private DropZone lastDropZone = null; // 記住上次放下的位置
    private CanvasGroup canvasGroup;
    private Vector3 dropOffset = new(0, -5.0f, 0);

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // 取得自己的位置
        canvas = GetComponentInParent<Canvas>(); // 取得自己所在的畫布
        canvasGroup = GetComponent<CanvasGroup>(); // 取得 CanvasGroup，方便後面使用
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition; // 開始拖曳的當下記住原本的位置
        canvasGroup.blocksRaycasts = false; // 拖曳中不阻擋滑鼠射線（讓 DropZone 能收到事件）
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 讓被拖曳物件隨著滑鼠移動，並確保拖曳不被畫面與 UI 縮放影響
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // 拖曳結束後恢復阻擋滑鼠射線
        if (DropZone.currentDragZone != null)
        {
            // 吸附到當前區域
            rectTransform.position = DropZone.currentDragZone.myPos.position + dropOffset;

            // 記住這次位置（該位置會成為下次的判斷起始點）
            lastDropZone = DropZone.currentDragZone;
        }
        else if (lastDropZone != null)
        {
            // 沒有吸附任何新地方，但有舊 DropZone，就吸回去
            rectTransform.position = lastDropZone.myPos.position + dropOffset;
        }
        else
        {
            // 完全沒進任何 DropZone，就回到最初原點
            rectTransform.anchoredPosition = originalPosition  + dropOffset;
        }
    }
}
