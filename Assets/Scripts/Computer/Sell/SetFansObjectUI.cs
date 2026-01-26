using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在販賣頁面的粉絲 prefab 上 */
public class SetFansObjectUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Image fansIcon;
    [SerializeField] private TextMeshProUGUI fansNameText;
    [SerializeField] private TextMeshProUGUI fansNumText;
    [SerializeField] private Image ownerIcon;
    //-----------------------------------------------------------------//
    [Header("角色粉絲資料")]
    private ItemStack fansItemStack;
    public ItemStack FansItemStack => fansItemStack;
    [SerializeField] private List<Sprite> ownerSprites = new(); // 依照 IdolWho enum 順序放置 
    
    public void Initialize(ItemStack fansItemStack, IdolWho idolIndex)
    {
        this.fansItemStack = fansItemStack;

        fansIcon.sprite = fansItemStack.item.icon;
        fansNameText.text = fansItemStack.item.itemName;
        fansNumText.text = $"{fansItemStack.quantity}";
        ownerIcon.sprite = ownerSprites[(int)idolIndex];
    }
}
