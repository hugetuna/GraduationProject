using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同
using System.Collections;

public enum DropZoneType { None, Member, Trainee } // 不受限於類別內

public class DragToLesson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static event Action<TrainingUIData> OnEnableOrEndDrag; // 拖曳結束或重啟 UI 事件
    //-----------------------------------------------------------------//
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    //-----------------------------------------------------------------//
    [Header("拖曳後的偏移")]
    [SerializeField] private Vector3 dropOffset = new(0f, 2f, 0f);
    private Vector2 originalPosition;
    private bool isDragging = false;
    private DropZone lastDropZone = null;
    private DropZoneType currentZoneType = DropZoneType.Member;
    public DropZoneType CurrentZoneType
    {
        get { return currentZoneType; }
    }
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private Slider vigourSlider;
    [SerializeField] private GameObject benefitBar;
    [SerializeField] private GameObject buffBoard;
    //-----------------------------------------------------------------//
    [Header("訓練 UI 資料")]
    [SerializeField] private TrainingUIData trainingUIData;
    private List<string> teamMembers = new();
    private List<string> teamTrainees = new();
    private string myName = null;
    private string MyName
    {
        get
        {
            if (string.IsNullOrEmpty(myName))
            {
                Image img = GetComponent<Image>();
                if (img != null && img.sprite != null)
                {
                    myName = img.sprite.name.Replace("UI_character_", "");
                }
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
    }

    void OnEnable()
    {
        StartCoroutine(InitAfterFrame());
    }

    private IEnumerator InitAfterFrame()
    {
        yield return null; // 等待所有物件完成 Start 以前的初始化，避免存取到 null 參考

        teamMembers = trainingUIData.teamData.GetMembers();
        teamTrainees = trainingUIData.teamData.GetTrainees();
        OnEnableOrEndDrag?.Invoke(trainingUIData); // 每次重啟 UI 就更新一次，確保介面狀態正確
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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

        if (DropZone.currentDragZone != null) // 拖曳成功，放到新的 DropZone
        {
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f); // 播放拖曳成功音效
            rectTransform.position = DropZone.currentDragZone.GetMyPos().position + dropOffset;
            lastDropZone = DropZone.currentDragZone;
        }
        else if (lastDropZone != null) // 拖曳失敗，回到上個 DropZone
        {
            rectTransform.position = lastDropZone.GetMyPos().position + dropOffset;
        }
        else // 拖曳失敗，回到原始位置（沒有上個 DropZone）
        {
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
        }

        UpdateTeamStatus(); // 更新隊伍狀態

        // 結束拖曳時，顯示角色底下的 UI 元素
        vigourSlider.gameObject.SetActive(true);
        benefitBar.SetActive(true);
        if (currentZoneType == DropZoneType.Member) buffBoard.SetActive(true);

        OnEnableOrEndDrag?.Invoke(trainingUIData); // 觸發拖曳結束事件
    }

    private void UpdateTeamStatus()
    {
        if (lastDropZone == null) return;

        if (lastDropZone.gameObject.name.Contains("m"))
        {
            if (!teamMembers.Contains(MyName))
            {
                teamMembers.Add(MyName);
                teamTrainees.Remove(MyName);
                currentZoneType = DropZoneType.Member;
            }
        }
        else if (lastDropZone.gameObject.name.Contains("t"))
        {
            if (!teamTrainees.Contains(MyName))
            {
                teamTrainees.Add(MyName);
                teamMembers.Remove(MyName);
                currentZoneType = DropZoneType.Trainee;
            }
        }
    }
}

