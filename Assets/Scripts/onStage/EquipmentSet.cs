using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquipmentSet : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image showIdol;
    public int stageSpot;
    public OnStageManager stageManager;
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    void Awake()
    {
        //紀錄位置
        originalPosition = transform.localPosition;
        stageManager = FindAnyObjectByType<OnStageManager>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //isDragging = true;

        // 紀錄位置
        originalPosition = transform.localPosition;
        //取消raycast阻擋
        canvasGroup.blocksRaycasts = false;

        // 調低透明度，增加視覺回饋
        canvasGroup.alpha = 0.6f;

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = originalPosition;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }
    public void OnDrop(PointerEventData eventData)
    {
        var idolDataList = stageManager.onStageIdols;
        EquipmentSet draggedSetUI = eventData.pointerDrag?.GetComponent<EquipmentSet>();

        if (draggedSetUI != null && draggedSetUI != this) // 確保不是放回自己身上
        {
            // 1. 先找出那兩個資料物件
            var idolA = idolDataList.Find(i => i.positionInTeam == this.stageSpot);
            var idolB = idolDataList.Find(i => i.positionInTeam == draggedSetUI.stageSpot);

            // 2. 交換資料層的數值 (如果資料存在的話)
            if (idolA != null) idolA.positionInTeam = draggedSetUI.stageSpot;
            if (idolB != null) idolB.positionInTeam = this.stageSpot;
            Debug.Log($"Swapped positions: IdolA is now at {idolA?.positionInTeam}, IdolB is now at {idolB?.positionInTeam}");
            // 3. 交換 UI 表現 (建議把這段包成 Function)
            SwapUI(this, draggedSetUI);
        }
    }

    private void SwapUI(EquipmentSet a, EquipmentSet b)
    {
        Sprite tempSprite = a.showIdol.sprite;
        a.showIdol.sprite = b.showIdol.sprite;
        b.showIdol.sprite = tempSprite;

        // 如果未來有名字、等級，也寫在這裡交換
        // string tempName = a.nameText.text; ...
    }
}
