using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableCloth : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int clothIndex; //衣服的索引
    public GameObject cloth;
    //元物件與位置
    private Transform originalParent;
    private Vector3 originalPosition;
    private void Start()
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
    }
    //拖曳
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        cloth.GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
        cloth.GetComponent<Image>().raycastTarget = true;
    }
}
