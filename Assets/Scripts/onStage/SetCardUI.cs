using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SetCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //讓休息及準備行為與卡片共用，建立一個是否為卡片的旗標
    public bool isCard;
    [Header("UI 元件")]
    public Image cardImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI voGateText;
    public TextMeshProUGUI daGateText;
    public TextMeshProUGUI viGateText;

    [Header("卡片資料")]
    public ActionCard cardData;
    //元物件與位置
    private Transform originalParent;
    private Vector3 originalPosition;
    //畫板
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    [Header("資訊框")]
    public GameObject infoPanel; // 拖進 Inspector
    public Vector3 hoverOffset = new Vector3(0, 30, 0); // 往上浮的距離
    [Header("拖曳區間")]
    public RectTransform dragLayer;
    private bool isDragging = false;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        dragLayer = GameObject.FindGameObjectWithTag("dragLayer").GetComponent<RectTransform>();
        //紀錄位置
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        
    }
    //紀錄原位置及卡片資訊
    public void SetCard(ActionCard cardToSet)
    {
        cardData = cardToSet;
        cardImage.sprite = cardData.cardPic;
        nameText.text = cardData.cardName;
        if (cardData.point == 0)
            pointText.text = " ";
        else
            pointText.text = cardData.point.ToString();
        durationText.text = cardData.applyDuration.ToString();
        voGateText.text = cardData.voGate.ToString();
        daGateText.text = cardData.daGate.ToString();
        viGateText.text = cardData.viGate.ToString();
    }

    //滑鼠進入與出
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isCard) return;//非卡片不顯示
        //紀錄位置
        originalPosition = transform.localPosition;
        if (isDragging) return; // 拖曳中不顯示
        transform.localPosition = originalPosition + hoverOffset;
        if (infoPanel != null)
            infoPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localPosition = originalPosition;
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    //拖曳
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        transform.localPosition = originalPosition;
        //紀錄位置
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        transform.SetParent(dragLayer, true);// 放到最上層避免被 UI 遮擋
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
        canvasGroup.blocksRaycasts = true;
    }
}
