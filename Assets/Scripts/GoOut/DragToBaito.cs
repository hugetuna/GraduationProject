using UnityEngine.EventSystems;
using UnityEngine;

public class DragToBaito : Drag
{
    private BaitoDropZone lastDropZone = null; // 紀錄上一次成功放置的 DropZone
    private BaitoDropZone currentDropZone = null; // 當前放置的 DropZone
    public BaitoDropZone CurrentDropZone
    {
        get { return currentDropZone; }
        set { currentDropZone = value; }
    }
    private IdolWho myIdolIndex;
    //-----------------------------------------------------------------//
    [Header("拖曳時受影響的 UI 元素")]
    [SerializeField] private GameObject vigourBar;
    private BaitoVigourBar vigourBarComponent;
    private BaitoNumbers numbersComponent;

    protected override void Awake()
    {
        base.Awake();
        vigourBarComponent = GetComponent<BaitoVigourBar>();
        numbersComponent = GetComponent<BaitoNumbers>();
    }

    public void Initialize(Baito data, IdolWho idolIndex, BaitoDropZone zone) // 僅在初次打開打工介面時呼叫一次 
    {
        myIdolIndex = idolIndex;
        lastDropZone = currentDropZone = zone;

        // 傳遞該角色名稱給其他元件進行初始化
        vigourBarComponent.Initialize(data, idolIndex);
        numbersComponent.Initialize(idolIndex);

        // 換場景 UI 就會重置，不用特別還原位置
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        if (lastDropZone != null)
        {
            // 清空上個 DropZone 的角色參考與裝備顯示
            var display = lastDropZone.GetComponent<BaitoEquipments>();
            if (display != null) display.UpdateEquipment();
            lastDropZone.ClearCurrentIdol();
        }

        // 開始拖曳時，隱藏角色其他 UI 元素
        vigourBar.SetActive(false);
    }

    // public void OnDrag(PointerEventData eventData); // 使用父類別的預設內容

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // 拖曳成功，放到新的 DropZone
        BaitoDropZoneType currentZoneType;
        if (currentDropZone != null) // 更新最後成功 DropZone
        {
            AudioManager.Instance.PlaySFX(dragCompletedSound, 0.5f); // 播放拖曳成功音效
            rectTransform.position = currentDropZone.MyRect.position + dropOffset;

            lastDropZone = currentDropZone;
            currentZoneType = currentDropZone.zoneType;
        }
        else if (lastDropZone != null) // 拖曳失敗，回到上個 DropZone
        {
            rectTransform.position = lastDropZone.MyRect.position + dropOffset;
            currentZoneType = lastDropZone.zoneType;
        }
        else // 拖曳失敗，沒有上個 DropZone，回到原始位置
        {
            rectTransform.anchoredPosition = originalPosition + (Vector2)dropOffset;
            currentZoneType = BaitoDropZoneType.Member;
        }

        // 結束拖曳時，顯示角色其他 UI 元素並適度更新
        if (lastDropZone != null)
        {
            lastDropZone.SetCurrentIdol(this);
            var display = lastDropZone.GetComponent<BaitoEquipments>();
            if (display != null) display.UpdateEquipment(myIdolIndex);
        }

        vigourBar.SetActive(true);
        vigourBarComponent.UpdateVigourBar(currentZoneType);
    }

}
