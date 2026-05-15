using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // UI 和物件的拖曳寫法不同

public class DragToLesson : Drag
{
    // [Header("位置資料")]
    // private bool isDragging = false;
    private DropZone lastDropZone = null; // 紀錄上一次成功放置的 DropZone
    private DropZone currentDropZone = null; // 當前放置的 DropZone
    public DropZone CurrentDropZone
    {
        get { return currentDropZone; }
        set { currentDropZone = value; }
    }
    private int zoneIndex; // 紀錄目前所在的 DropZone 編號
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private GameObject vigourSlider;
    private TrainingVigourBar vigourBar; // 對應腳本參考
    //-----------------------------------------------------------------//
    // private TrainingUIData trainingUIData; // 目前訓練 UI 的資料
    private IdolWho myIdolIndex;
    public IdolWho MyIdolIndex { get { return myIdolIndex; } }

    protected override void Awake()
    {
        base.Awake();

        vigourBar = GetComponent<TrainingVigourBar>();
    }

    public void Initialize(IdolWho idolIndex) // 僅在初次打開訓練介面時呼叫一次 
    {
        myIdolIndex = idolIndex;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (lastDropZone != null)
        {
            // 清空上個 DropZone 的角色參考與裝備顯示
            var display = lastDropZone.GetComponent<SetEquipmentUI>();
            if (display != null) display.UpdateEquipment();
        }

        // 開始拖曳時，隱藏角色底下的 UI 元素
        vigourSlider.SetActive(false);
    }

    // public void OnDrag(PointerEventData eventData); // 使用父類別的預設內容

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // 拖曳成功，放到新的 DropZone
        DropZoneType currentZoneType;
        if (CurrentDropZone != null) // 更新最後成功 DropZone
        {
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f); // 播放拖曳成功音效
            rectTransform.position = CurrentDropZone.MyRect.position + dropOffset;

            lastDropZone = CurrentDropZone;
            currentZoneType = CurrentDropZone.zoneType;
            zoneIndex = CurrentDropZone.zoneIndex;

            // 第一天指定角色訓練的特殊情形
            if (DayManager.Instance != null && DayManager.Instance.day == 1)
            {
                var currentEvent = DayManager.Instance.dayEventManager.currentEvent;
                if (currentEvent != null)
                {
                    if(currentEvent.TriggerTimeIndex == 6 && currentEvent.targetIdol == MyIdolIndex)
                    {
                        // 角色放到訓練室後就不允許再移動了
                        GetComponent<CanvasGroup>().blocksRaycasts = false;
                    }
                }
            }
        }
        else if (lastDropZone != null) // 拖曳失敗，回到上個 DropZone
        {
            rectTransform.position = lastDropZone.MyRect.position + dropOffset;
            currentZoneType = lastDropZone.zoneType;
            zoneIndex = lastDropZone.zoneIndex;
        }
        else // 拖曳失敗，沒有上個 DropZone，回到原始位置
        {
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
            currentZoneType = DropZoneType.Member;
            // 不動 zoneIndex
        }
        UpdateTeamStatus(currentZoneType);

        // 更新角色底下的 UI 元素
        if (lastDropZone != null)
        {
            // lastDropZone.SetCurrentIdol(this);
            var display = lastDropZone.GetComponent<SetEquipmentUI>();
            if (display != null) display.UpdateEquipment(myIdolIndex);
        }

        vigourSlider.SetActive(true);
        vigourBar.UpdateVigourBar(currentZoneType); // 根據新的區域類型更新體力條顯示

        // 同步更新 IdolInstance 的 trainRecord（備份用）
        TraineeAssignment.UpdateIdolTrainRecord(
            MyIdolIndex,
            position: rectTransform.anchoredPosition,
            droppedZoneType: currentZoneType,
            droppedZoneIndex: zoneIndex
        );
    }

    private void UpdateTeamStatus(DropZoneType newZoneType)
    {
        IdolTrainingState newState = newZoneType switch
        {
            DropZoneType.Member => IdolTrainingState.InTeam,
            DropZoneType.Dance => IdolTrainingState.InDance,
            DropZoneType.Vocal => IdolTrainingState.InVocal,
            DropZoneType.Visual => IdolTrainingState.InVisual,
            _ => IdolTrainingState.None
        };

        TrainingUIManager.Instance.SetIdolState(MyIdolIndex, newState);
    }
}

