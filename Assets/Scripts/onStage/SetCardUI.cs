using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SetCardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //讓休息及準備行為與卡片共用，建立一個是否為卡片的旗標
    [Header("狀態控制")]
    public bool isInteractive = true; // true = 可以拖曳; false = 僅展示
    public bool isCard;
    [Header("UI 素材表")]
    public List<Sprite> numberSprites;
    public List<Sprite> numberSpritesForVigor;
    public List<Sprite> backGroundSprites;
    [Header("UI 元件")]
    public Image backgroundImage;
    public Image cardImage;
    public TextMeshProUGUI nameText;
    //public TextMeshProUGUI pointText;
    //public TextMeshProUGUI durationText;
    //public TextMeshProUGUI vigorCostText;
    public Image pointText_thousand;
    public Image pointText_hundred;
    public Image pointText_ten;
    public Image pointText_one;
    public Image durationText;
    public Image vigorCostText;
    public TextMeshProUGUI voGateText;
    public TextMeshProUGUI daGateText;
    public TextMeshProUGUI viGateText;
    //發光效果
    public Image glowEffect;
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
    public SetInfoCard infoPanel; // 拖進 Inspector
    public Vector3 hoverOffset = new Vector3(0, 30, 0); // 往上浮的距離
    [Header("拖曳區間")]
    public RectTransform dragLayer;
    private bool isDragging = false;

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        //紀錄位置
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
    }
    public void ShowGlowEffect(bool show)
    {
        if (glowEffect != null)
        {
            glowEffect.gameObject.SetActive(show);
        }
    }
    //紀錄原位置及卡片資訊
    public void SetCard(ActionCard cardToSet)
    {
        if (isInteractive&&isCard)
        {
            dragLayer = GameObject.FindGameObjectWithTag("dragLayer").GetComponent<RectTransform>();
            infoPanel = GameObject.FindGameObjectWithTag("InfoCardInInGame").GetComponent<SetInfoCard>();
        }
        else if (isCard&&!isInteractive)
        {
            infoPanel = GameObject.FindGameObjectWithTag("InfoCardInGameStartPanel").GetComponent<SetInfoCard>();
        }
        cardData = cardToSet;
        backgroundImage.sprite= backGroundSprites[(int)cardData.cardType];
        cardImage.sprite = cardData.cardPic;
        nameText.text = cardData.cardName;
        //if (cardData.point == 0)
        //    pointText.text = " ";
        //else
        //    pointText.text = cardData.point.ToString();
        //durationText.text = cardData.applyDuration.ToString();
        //vigorCostText.text = cardData.staminaCost.ToString();
        setPoint(cardData.point);
        durationText.sprite= numberSprites[(int)cardData.applyDuration];
        vigorCostText.sprite = numberSpritesForVigor[(int)cardData.staminaCost];
        voGateText.text = cardData.voGate.ToString();
        daGateText.text = cardData.daGate.ToString();
        viGateText.text = cardData.viGate.ToString();
    }
    public void setPoint(int point)
    {
        if(cardData.point == 0)
        {
            pointText_thousand.gameObject.SetActive(false);
            pointText_hundred.gameObject.SetActive(false);
            pointText_ten.gameObject.SetActive(false);
            pointText_one.gameObject.SetActive(false);
        }
        else
        {
            string pointStr = point.ToString("D4"); // 轉換為4位數字，不足補0
            Debug.Log(pointStr);
            pointText_thousand.sprite = numberSprites[int.Parse(pointStr[0].ToString())];
            pointText_hundred.sprite = numberSprites[int.Parse(pointStr[1].ToString())];
            pointText_ten.sprite = numberSprites[int.Parse(pointStr[2].ToString())];
            pointText_one.sprite = numberSprites[int.Parse(pointStr[3].ToString())];
            //確保數字圖片顯示
            if (int.Parse(pointStr[0].ToString()) == 0)
            {
                pointText_thousand.gameObject.SetActive(false);
            }else
            {
                pointText_thousand.gameObject.SetActive(true);
            }
            pointText_hundred.gameObject.SetActive(true);
            pointText_ten.gameObject.SetActive(true);
            pointText_one.gameObject.SetActive(true);
        }
    }
    //滑鼠進入與出
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isCard) return;//非卡片不顯示
        //紀錄位置
        if (isDragging) return; // 拖曳中不顯示
        originalPosition = transform.localPosition;
        if (isInteractive)
        {
            transform.localPosition = originalPosition + hoverOffset;
        }
        if (infoPanel != null)
            infoPanel.SetInfo(cardData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localPosition = originalPosition;
        if (infoPanel != null)
            infoPanel.ClearInfo();
    }

    //拖曳
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInteractive) return;
        isDragging = true;
        
        // 恢復原位 (避免 hover offset 干擾)
        transform.localPosition = originalPosition;

        // 紀錄位置
        originalParent = transform.parent;
        originalPosition = transform.localPosition;

        // 進入拖曳層
        transform.SetParent(dragLayer, true);
        canvasGroup.blocksRaycasts = false;

        //立刻將卡片移到滑鼠中心
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragLayer,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        rectTransform.anchoredPosition = localPoint;
        //提示能夠被放置的偶像
        OnStageManager onStageManager = FindAnyObjectByType<OnStageManager>();
        if (onStageManager != null)
        {
            foreach (IdolInstance idol in onStageManager.onStageIdols)
            {
                IdolOnStage idolOnStage = idol.GetComponent<IdolOnStage>();
                if (idolOnStage.StageVocal >=cardData.voGate && idolOnStage.StageDance >= cardData.daGate && idolOnStage.StageVisual >= cardData.viGate)
                {
                    idolOnStage.ShowUseableIndicator(true);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteractive) return; // 不可拖曳就跳出
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isInteractive) return; // 不可拖曳就跳出
        isDragging = false;
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
        canvasGroup.blocksRaycasts = true;
        //還原所有放置提示
        OnStageManager onStageManager = FindAnyObjectByType<OnStageManager>();
        if (onStageManager != null)
        {
            foreach (IdolInstance idol in onStageManager.onStageIdols)
            {
                IdolOnStage idolOnStage = idol.GetComponent<IdolOnStage>();
                idolOnStage.ShowUseableIndicator(false);
            }
        }
    }
}
