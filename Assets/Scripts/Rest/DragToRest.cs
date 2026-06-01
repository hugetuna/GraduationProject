using UnityEngine;
using UnityEngine.EventSystems;

public class DragToRest : Drag
{
    private RestDropZone lastDropZone = null; // 紀錄上一次成功放置的 DropZone
    private RestDropZone currentDropZone = null; // 當前放置的 DropZone
    public RestDropZone CurrentDropZone
    {
        get { return currentDropZone; }
        set { currentDropZone = value; }
    }
    private IdolWho myIdolIndex;
    public IdolWho MyIdolIndex { get { return myIdolIndex; } }
    private IdolInstance characterInfo; // 方便存取用
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private GameObject vigourBar;
    private RestVigourBar vigourBarComponent;

    protected override void Awake()
    {
        base.Awake();
        vigourBarComponent = GetComponent<RestVigourBar>();
    }

    public void Initialize(IdolWho idolIndex, RestDropZone zone) // 每次打開休息介面時呼叫一次 
    {
        myIdolIndex = idolIndex;
        characterInfo = TeamDataUtility.IdolDict[myIdolIndex];
        lastDropZone = currentDropZone = zone;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        // 開始拖曳時，隱藏角色其他 UI 元素
        vigourBar.SetActive(false);
    }

    // public void OnDrag(PointerEventData eventData); // 使用父類別的預設內容

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // 拖曳成功，放到新的 DropZone
        RestDropZoneType currentZoneType;
        bool success = false;
        if (currentDropZone != null) // 更新最後成功 DropZone
        {
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f); // 播放拖曳成功音效
            rectTransform.position = currentDropZone.MyRect.position + dropOffset;

            lastDropZone = currentDropZone;
            currentZoneType = currentDropZone.zoneType;
            success = true;
        }
        else if (lastDropZone != null) // 拖曳失敗，回到上個 DropZone
        {
            rectTransform.position = lastDropZone.MyRect.position + dropOffset;
            currentZoneType = lastDropZone.zoneType;
        }
        else // 拖曳失敗，沒有上個 DropZone，回到原始位置 => 理論上不會發生，除非一開始沒有正確初始化
        {
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
            currentZoneType = RestDropZoneType.Member;
        }

        // 結束拖曳時，顯示角色其他 UI 元素並適度更新
        vigourBar.SetActive(true);
        vigourBarComponent.UpdateVigourBar(currentZoneType);

        // 跨場景同步
        characterInfo.restRecord.SetRestRecord(
            /*position:*/ rectTransform.anchoredPosition, // 更新位置
            /*zoneType:*/ currentZoneType, // 更新區域類型
            /*zoneIndex:*/ success ? currentDropZone.zoneIndex : lastDropZone.zoneIndex // 更新區域索引
        );
    }

}
