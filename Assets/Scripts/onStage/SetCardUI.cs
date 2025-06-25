using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SetCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI 元件")]
    public Image cardImage;
    public TextMeshProUGUI pointText;

    [Header("卡片資料")]
    public ActionCard cardData;
    //元物件與位置
    private Transform originalParent;
    private Vector3 originalPosition;
    //畫板
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetCard(ActionCard cardToSet)
    {
        cardData = cardToSet;
        cardImage.sprite = cardData.cardPic;

        if (cardData.point == 0)
            pointText.text = " ";
        else
            pointText.text = cardData.point.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        transform.SetParent(canvas.transform); // 放到最上層避免被 UI 遮擋
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        canvasGroup.blocksRaycasts = true;
    }
}
