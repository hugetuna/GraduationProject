using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

public class DragToLesson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; // UI 元件在畫布上的位置
    private Canvas canvas; // 畫布本身

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>(); // 取得 UI 元件在畫布上的位置
        canvas = GetComponentInParent<Canvas>(); // 找到這個 UI 元件所在的畫布
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("開始拖曳");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 使用畫布的 scale 做補償，確保拖曳不會跑掉
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("結束拖曳");
    }
}
