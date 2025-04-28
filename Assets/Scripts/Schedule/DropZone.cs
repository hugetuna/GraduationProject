using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static DropZone currentDragZone;

    public RectTransform myPos;

    private void Start()
    {
        myPos = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData) // 當滑鼠將角色拖曳過來
    {
        currentDragZone = this;
    }

    public void OnPointerExit(PointerEventData eventData) // 當滑鼠將角色拖曳離開
    {
        if (currentDragZone == this) currentDragZone = null;
    }
}
