using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

public enum DropZoneType { None, Member, Trainee } // 不受限於類別內
public class DragToLesson : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition;
    private DropZone lastDropZone = null; // 記住上次放下的位置
    public DropZoneType currentZoneType = DropZoneType.Member; // 代表當前拖放區域
    private CanvasGroup canvasGroup;
    [SerializeField] private Vector2 dropOffset = new(0, -3.0f);
    private bool isDragging = false; // 是否正在拖曳

    public TeamUIData teamUIData;  // 透過 ScriptableObject 取得當前隊伍 UI 資料
    private List<string> teamMembers;
    private List<string> teamTrainees;
    private UnityEngine.UI.Image image;
    private string myName;
    public Slider vigourSlider; // 該角色的體力值 UI
    public VigourBar vigourBar;
    public GameObject benefitBar; // 該角色的訓練收益 UI
    public GameObject buffBoard; // 該角色的訓練buff UI

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>(); // 取得角色 UI 自己的位置
        canvas = GetComponentInParent<Canvas>(); // 取得自己所在的畫布
        canvasGroup = GetComponent<CanvasGroup>(); // 取得 CanvasGroup，方便後面使用

        teamMembers = teamUIData.teamMembers;
        teamTrainees = teamUIData.teamTrainees;
        image = GetComponent<UnityEngine.UI.Image>();
        myName = image.sprite.name.Replace("UI_character_", ""); ; // 取得該角色的來源圖片名稱（不含副檔名）

        vigourBar = GetComponent<VigourBar>(); // 取得該角色的 VigourBar 參考
    }

    private void Update()
    {
        // 判斷該角色 UI 目前在 Member 區還是 Trainee 區
        if (lastDropZone != null)
        {
            if (lastDropZone.gameObject.name.Contains("m"))
            {
                if (!teamMembers.Contains(myName))
                {
                    teamMembers.Add(myName); // 將該角色列入當前隊伍
                    teamTrainees.Remove(myName); // 並移出訓練名單
                    currentZoneType = DropZoneType.Member; // 更新當前拖放區域名稱
                }
            }
            else if (lastDropZone.gameObject.name.Contains("t"))
            {
                if (!teamTrainees.Contains(myName))
                {
                    teamTrainees.Add(myName); // 將該角色列入訓練名單
                    teamMembers.Remove(myName); // 並移出當前隊伍
                    currentZoneType = DropZoneType.Trainee; // 更新當前拖放區域名稱
                }
            }

            // // 因為不能讓物件自己控制自己的可用狀態，所以寫在 DragToLsseon 裡面
            if (!isDragging) // 沒在拖曳時才判斷
            {
                if (currentZoneType == DropZoneType.Trainee)
                {
                    if (buffBoard.activeSelf) buffBoard.SetActive(false);
                }
                else if (currentZoneType == DropZoneType.Member)
                {
                    if (!buffBoard.activeSelf) buffBoard.SetActive(true);
                }

            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (vigourBar.isAbleToTrain)
        {
            isDragging = true; // 開始拖曳

            originalPosition = rectTransform.anchoredPosition; // 開始拖曳的當下記住原本的位置
            canvasGroup.blocksRaycasts = false; // 拖曳中不阻擋滑鼠射線（讓 DropZone 能收到事件）

            vigourSlider.gameObject.SetActive(false); // 拖曳時隱藏體力值 UI
            benefitBar.SetActive(false); // 拖曳時隱藏訓練收益 UI
            buffBoard.SetActive(false); // 拖曳時隱藏buff UI
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (vigourBar.isAbleToTrain)
        {
            // 讓被拖曳物件隨著滑鼠移動，並確保拖曳不被畫面與 UI 縮放影響
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (vigourBar.isAbleToTrain)
        {
            isDragging = false; // 結束拖曳

            canvasGroup.blocksRaycasts = true; // 拖曳結束後恢復阻擋滑鼠射線
            vigourSlider.gameObject.SetActive(true); // 拖曳後恢復體力值 UI 的顯示
            benefitBar.SetActive(true); // 拖曳後恢復訓練收益 UI 的顯示

            RectTransform parentRect = rectTransform.parent as RectTransform;
            Vector2 screenPos, localPoint;
            bool isSuccess; // 紀錄座標是否轉換成功
                            // 判斷最終放置位置
            if (DropZone.currentDragZone != null)
            {
                // 取得放置點在螢幕上的座標
                screenPos = RectTransformUtility.WorldToScreenPoint(
                        eventData.pressEventCamera,
                        DropZone.currentDragZone.myPos.position
                    );

                // 轉換為本地 anchoredPosition
                isSuccess =
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect,
                        screenPos,
                        eventData.pressEventCamera,
                        out localPoint
                    );
                if (isSuccess) rectTransform.anchoredPosition = localPoint + dropOffset; // 吸附到新位置
                lastDropZone = DropZone.currentDragZone; // 記住這次的位置
            }
            else if (lastDropZone != null)
            {
                // 取得放置點在螢幕上的座標
                screenPos = RectTransformUtility.WorldToScreenPoint(
                        eventData.pressEventCamera,
                        lastDropZone.myPos.position
                    );

                // 轉換為本地 anchoredPosition
                isSuccess =
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRect,
                        screenPos,
                        eventData.pressEventCamera,
                        out localPoint
                    );
                // 沒有吸附任何新地方，但有舊 DropZone，就吸回去
                if (isSuccess) rectTransform.anchoredPosition = localPoint + dropOffset;
            }
            else
            {
                // 完全沒進任何 DropZone，就回到最初原點
                rectTransform.anchoredPosition = originalPosition + dropOffset;
            }
        }
    }
}
