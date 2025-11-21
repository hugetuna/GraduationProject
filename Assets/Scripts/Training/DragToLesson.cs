using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

public class DragToLesson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    //-----------------------------------------------------------------//
    [Header("拖曳後的偏移")]
    [SerializeField] private Vector3 dropOffset = new(0f, 2f, 0f);
    private Vector2 originalPosition;
    //-----------------------------------------------------------------//
    private bool isDragging = false;
    private DropZone lastDropZone = null; // 紀錄上一次成功放置的 DropZone
    private DropZone currentDropZone = null; // 當前放置的 DropZone
    public DropZone CurrentDropZone
    {
        get { return currentDropZone; }
        set { currentDropZone = value; }
    }
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private Slider vigourSlider;
    [SerializeField] private GameObject benefitBar;
    [SerializeField] private GameObject buffBoard;
    private VigourBar vigourBar; // 對應腳本參考
    private BenefitBar benefitBarComp;
    private BuffBoard buffBoardComp;
    //-----------------------------------------------------------------//
    // [Header("訓練 UI 資料")]
    private TrainingUIData trainingUIData;
    private string myName = "";
    private string MyName
    {
        get
        {
            if (string.IsNullOrEmpty(myName))
            {
                Image img = GetComponent<Image>();
                if (img != null && img.sprite != null)
                {
                    myName = TeamDataUtility.CleanNameOfCharacterUI(img.sprite.name);
                }
                else return "";
            }
            return myName;
        }
    }
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip dragCompletedSound; // 拖曳成功的音效

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        vigourBar = GetComponent<VigourBar>();
        benefitBarComp = GetComponent<BenefitBar>();
        buffBoardComp = GetComponent<BuffBoard>();

        originalPosition = rectTransform.anchoredPosition;
    }

    public void Initialize(TrainingUIData data) // 僅在初次打開訓練介面時呼叫一次 
    {
        trainingUIData = data;

        // 傳遞該角色名稱給底下的元件進行初始化
        vigourBar.Initialize(MyName);
        benefitBarComp.Initialize(MyName, trainingUIData);
        buffBoardComp.Initialize(MyName);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup.interactable == false)
        {
            Debug.Log("無法拖曳");
            return;
        }

        isDragging = true;
        canvasGroup.blocksRaycasts = false;

        // 開始拖曳時，隱藏角色底下的 UI 元素
        vigourSlider.gameObject.SetActive(false);
        benefitBar.SetActive(false);
        buffBoard.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 將滑鼠位置轉成世界座標，直接設置物件位置
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 globalMousePos))
        {
            rectTransform.position = globalMousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        // 拖曳成功，放到新的 DropZone
        DropZoneType currentZoneType;
        if (CurrentDropZone != null) // 更新最後成功 DropZone
        {
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f); // 播放拖曳成功音效
            rectTransform.position = CurrentDropZone.MyRect.position + dropOffset;

            lastDropZone = CurrentDropZone;
            currentZoneType = CurrentDropZone.zoneType;

            // 更新底下元件的資料
            vigourBar.UpdateVigourBar(trainingUIData);
            benefitBarComp.UpdateBenefitBar(trainingUIData);
            buffBoardComp.UpdateBuffBoard(trainingUIData);
        }
        else if (lastDropZone != null) // 拖曳失敗，回到上個 DropZone
        {
            rectTransform.position = lastDropZone.MyRect.position + dropOffset;
            currentZoneType = lastDropZone.zoneType;
        }
        else // 拖曳失敗，沒有上個 DropZone，回到原始位置
        {
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
            currentZoneType = DropZoneType.Member;
        }
        UpdateTeamStatus(currentZoneType);

        // 結束拖曳時，顯示角色底下的 UI 元素
        vigourSlider.gameObject.SetActive(true);
        benefitBar.SetActive(true);
        if (currentZoneType == DropZoneType.Member) buffBoard.SetActive(true);
    }

    private void UpdateTeamStatus(DropZoneType newZoneType)
    {
        IdolTrainingState newState = newZoneType switch
        {
            DropZoneType.Member => IdolTrainingState.InTeam,
            DropZoneType.Dance => IdolTrainingState.InDance,
            DropZoneType.Vocal => IdolTrainingState.InVocal,
            DropZoneType.Visual => IdolTrainingState.InVisual,
            _ => IdolTrainingState.InTeam
        };
        
        TrainingUIManager.Instance.SetIdolState(MyName, newState);
    }
}

