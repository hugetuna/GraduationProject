using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventoryManager : MonoBehaviour
{
    // 遊戲中所有可用的裝備清單
    public List<EquipmentItem> allEquipments;
    // 玩家擁有的所有裝備（裡面可能有重複的 SO）
    public List<EquipmentItem> ownedEquipments = new List<EquipmentItem>();
    public EquipmentItem FindEquipmentByName(string equipmentName)
    {
        return allEquipments.Find(x => x.itemName == equipmentName);
    }
    public void TryEquip(EquipmentItem itemToEquip, IdolInstance targetIdol)
    {
        // 1. 計算這件裝備玩家總共有幾件
        int totalOwned = ownedEquipments.FindAll(x => x == itemToEquip).Count;
        // 2. 計算目前全隊伍已經穿了幾件這款裝備
        int totalInUse = 0;
        if (FindAnyObjectByType<TeamManager>())
        {
            FindAnyObjectByType<TeamManager>().gameObject.GetComponent<TeamManager>().teamMembers.ForEach(x =>
            {
                IdolInstance idol = x.GetComponent<IdolInstance>();
                if (idol.equipmentItemNow == itemToEquip)
                {
                    totalInUse++;
                }
            });
        }
        else if(FindAnyObjectByType<OnStageManager>())
        {
            FindAnyObjectByType<OnStageManager>().gameObject.GetComponent<OnStageManager>().onStageIdols.ForEach(x =>
            {
                if (x.equipmentItemNow == itemToEquip)
                {
                    totalInUse++;
                }
            });
        }
        // 3. 判斷是否還有剩餘的
        if (totalInUse < totalOwned)
        {
            // 如果這偶像原本有穿別的，先脫掉（不影響庫存數量，只影響占用數）
            targetIdol.equipmentItemNow = itemToEquip;
            targetIdol.equippedItemName= itemToEquip.itemName;
            Debug.Log($"{targetIdol.name} 穿上了 {itemToEquip.itemName}");
        }
        else
        {
            Debug.LogWarning("裝備數量不足！去商店再買一件吧。");
        }
    }
    public void Unequip(IdolInstance targetIdol)
    {
        if (targetIdol.equipmentItemNow != null)
        {
            Debug.Log($"{targetIdol.name} 脫下了 {targetIdol.equipmentItemNow.itemName}");
            targetIdol.equipmentItemNow = null;
        }
        else
        {
            Debug.LogWarning($"{targetIdol.name} 沒有裝備可脫下！");
        }
    }
}
