using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在道具項目的 prefab 根部（Button） */
public class SetItemUI : MonoBehaviour
{
    private Item item; // 道具資料
    private int quantity; // 道具數量
    public Item Item => item;
    public int Quantity => quantity;
    //-----------------------------------------------------------------//
    private TextMeshProUGUI itemNameText; // 顯示道具名稱的 UI 元素
    private TextMeshProUGUI itemStackText; // 顯示道具數量的 UI 元素
    private Image itemIcon; // 顯示道具圖示的 UI 元素
    private Image fansOwnerIcon; // 粉絲道具專用，顯示持有者的圖示

    void Awake()
    {
        itemNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        itemStackText = transform.Find("StackText").GetComponent<TextMeshProUGUI>();
        itemIcon = transform.Find("Image").GetComponent<Image>();
        fansOwnerIcon = transform.Find("FansOwner").GetComponent<Image>();
    }

    public void Initialize(Item newItem, int newQuantity, IdolWho fansOwner = IdolWho.none)
    {
        item = newItem;
        quantity = newQuantity;

        itemNameText.text = item.itemName;
        itemStackText.text = "x" + quantity.ToString();
        itemIcon.sprite = item.icon;

        // 處理粉絲道具的持有者圖示
        if (fansOwner != IdolWho.none && TeamDataUtility.IdolDict.ContainsKey(fansOwner))
        {
            fansOwnerIcon.gameObject.SetActive(true);
            fansOwnerIcon.sprite = TeamDataUtility.IdolDict[fansOwner].idolUISprites.spriteIcon;
        }
        else
        {
            fansOwnerIcon.gameObject.SetActive(false);
        }

        // 確保字型正確渲染
        itemNameText.ForceMeshUpdate(); 
        itemStackText.ForceMeshUpdate();
    }
}
