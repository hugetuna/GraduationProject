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
    private int zoneIndex = -1; // 紀錄目前所在的 DropZone 編號
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private GameObject vigourSlider;
    private VigourBar vigourBar; // 對應腳本參考
    //-----------------------------------------------------------------//
    private TrainingUIData trainingUIData; // 目前訓練 UI 的資料
    private string myName = "";
    private string MyName
    {
        get
        {
            if (string.IsNullOrEmpty(myName))
            {
                myName = GetComponent<Image>().sprite?.name ?? "";
                myName = TeamDataUtility.CleanNameOfCharacterUI(myName);
            }
            return myName;
        }
    }
    private IdolWho MyIdolIndex => TeamDataUtility.GetIdolEnum(MyName);

    protected override void Awake()
    {
        base.Awake();

        vigourBar = GetComponent<VigourBar>();
        // numbersController = GetComponentInParent<NumbersController>();
    }

    public void Initialize(TrainingUIData data) // 僅在初次打開訓練介面時呼叫一次 
    {
        trainingUIData = data;

        // 傳遞該角色名稱給底下的元件進行初始化
        vigourBar.Initialize(trainingUIData, MyIdolIndex);

        // 確保換場景後 UI 不會跑掉
        var idol = TeamDataUtility.IdolDict[MyIdolIndex];
        if (idol.trainRecord.IsInTeamScope())
        {
            vigourSlider.SetActive(true);
        }
        else vigourSlider.SetActive(false);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

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

            // 清掉原本位置的數值資料
            // if(lastDropZone == null)
            // {
            //     var idol = TeamDataUtility.IdolDict[MyIdolIndex];
            //     var tr = idol.trainRecord;
            //     numbersController.ClearSlot(tr.droppedZoneType, tr.droppedZoneIndex);
            //     Debug.Log($"清除原本位置的數值資料: {tr.droppedZoneType}, {tr.droppedZoneIndex}");
            // }
            // else
            // {
            //     numbersController.ClearSlot(lastDropZone.zoneType, zoneIndex);
            //     Debug.Log($"清除原本位置的數值資料: {lastDropZone.zoneType}, {zoneIndex}");
            // }

            lastDropZone = CurrentDropZone;
            currentZoneType = CurrentDropZone.zoneType;
            zoneIndex = CurrentDropZone.zoneIndex;

            // 更新當前位置的數值資料
            NumbersController.NotifyIdolMoved(
                MyIdolIndex,
                currentZoneType,
                zoneIndex,
                trainingUIData
            );
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
        vigourBar.UpdateVigourBar();
        if (currentZoneType == DropZoneType.Member) vigourSlider.SetActive(true);
        else vigourSlider.SetActive(false);

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

