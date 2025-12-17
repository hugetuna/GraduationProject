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
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    void Awake()
    {
        //紀錄位置
        originalPosition = transform.localPosition;
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
        var idolDataList = GameManager.Instance.idolDataList;
        // 嘗試從拖曳來源取得 EquipmentSet
        EquipmentSet draggedSetUI = eventData.pointerDrag?.GetComponent<EquipmentSet>();
        if (draggedSetUI != null && draggedSetUI.stageSpot != stageSpot) {
            // 交換兩個 EquipmentSet 的 showIdol 圖片
            Sprite tempSprite = showIdol.sprite;
            showIdol.sprite = draggedSetUI.showIdol.sprite;
            draggedSetUI.showIdol.sprite = tempSprite;

            // 交換GameManager中的PositionInTeam
            foreach (var idol in idolDataList)
            {
                //找到被放的，將其設為被拖的位置
                if (idol.positionInTeam == stageSpot)
                {
                    idol.positionInTeam = draggedSetUI.stageSpot;
                }
                //找到被拖的，將其設為被放的位置
                else if (idol.positionInTeam == draggedSetUI.stageSpot)
                {
                    idol.positionInTeam = stageSpot;
                }
            }
        }
    }
}
