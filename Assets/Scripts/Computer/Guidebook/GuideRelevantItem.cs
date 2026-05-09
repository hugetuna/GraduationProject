using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* 掛在圖鑑視窗的相關道具物件上 */
public class GuideRelevantItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon; // 道具圖示
    //------------------------------------------------------------------//
    private bool canShowHover = false;
    [SerializeField] private GameObject hoverObj; // hover 提示物件
    [SerializeField] private TextMeshProUGUI hoverText; // 其中的道具名稱
    [SerializeField] private TextMeshProUGUI hoverDesc; // 其中的道具描述

    public void UpdateDisplay(Item item)
    {
        icon.sprite = item.icon;

        canShowHover = true;
        hoverText.text = item.itemName;
        hoverDesc.text = item.description;
    }

    // 當滑鼠游標「進入」物件範圍時觸發 hover 效果
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canShowHover) hoverObj.SetActive(true);
    }

    // 當滑鼠游標「離開」物件範圍時觸發
    public void OnPointerExit(PointerEventData eventData)
    {
        if (canShowHover) hoverObj.SetActive(false);
    }

    public void ClearDisplay()
    {
        icon.sprite = null;
        
        hoverObj.SetActive(false);
        canShowHover = false;
        hoverText.text = "";
        hoverDesc.text = "";
    }
}
