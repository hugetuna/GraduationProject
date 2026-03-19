using UnityEngine;
using UnityEngine.UI;

/* 掛在有裝備槽的 DropZone 上 */
public class BaitoEquipments : MonoBehaviour
{
    [SerializeField] private Image equipmentSlot; // 這格對應的裝備槽（圖片）

    void Start()
    {
        UpdateEquipment(); // 初始狀態先清空裝備槽
    }

    public void UpdateEquipment(IdolWho idolIndex = IdolWho.none)
    {
        if (idolIndex == IdolWho.none)
        {
            equipmentSlot.sprite = null; // 沒有角色，清空裝備槽
            return;
        }

        var idol = TeamDataUtility.IdolDict[idolIndex];
        if (idol != null && idol.equipmentItemNow != null)
        {
            equipmentSlot.sprite = idol.equipmentItemNow.icon;
        }
        else equipmentSlot.sprite = null;

    }
}
