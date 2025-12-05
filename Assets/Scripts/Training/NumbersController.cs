using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class StatsSlot
{
    public TextMeshProUGUI fans;
    public TextMeshProUGUI dance;
    public TextMeshProUGUI vocal;
    public TextMeshProUGUI visual;
    public List<GameObject> buffList; // 顯示加成效果的物件列表（底下的文字和圖示另外設定）
    public IdolInstance currentIdol; // 這個 slot 目前放哪個角色
}

/* 控制角色拖曳後的數值顯示，預計會掛在不同的訓練 UI 上 */
public class NumbersController : MonoBehaviour
{
    [SerializeField] private List<StatsSlot> memberSlots = new();
    [SerializeField] private List<StatsSlot> traineeSlots = new();

    public void InitializeSlots(TrainingUIData data)
    {
        // 先全部清空
        foreach (var slot in memberSlots)
        {
            slot.currentIdol = null;
            slot.fans.text = "";
            slot.dance.text = "";
            slot.vocal.text = "";
            slot.visual.text = "";
            foreach (var buff in slot.buffList)
            {
                buff.SetActive(false);
            }
        }

        foreach (var slot in traineeSlots)
        {
            slot.currentIdol = null;
            slot.fans.text = "";
            slot.dance.text = "";
            slot.vocal.text = "";
            slot.visual.text = "";
            foreach (var buff in slot.buffList)
            {
                buff.SetActive(false);
            }
        }

        // 取得所有成員
        var idols = TeamDataUtility.IdolInstanceList;

        for (int i = 0; i < idols.Count; i++)
        {
            var idol = idols[i];
            var trainRecord = idol.trainRecord;
            
            if (i < memberSlots.Count) // 避免超出範圍
            {
                // 設定各角色初始所在 slot
                AssignIdolSlot(idol.idolIndex, trainRecord.droppedZoneType, trainRecord.droppedZoneIndex, data);
            }
        }
    }


    public void AssignIdolSlot(IdolWho idolIndex, DropZoneType zoneType, int slotIndex, TrainingUIData data)
    {
        var idol = TeamDataUtility.IdolDict[idolIndex];

        var slotList = zoneType == DropZoneType.Member ? memberSlots : traineeSlots;
        var slot = slotList[slotIndex];

        slot.currentIdol = idol; // 記錄這個 slot 的角色

        // 更新數值（先預設為有老師加成）
        slot.fans.text = idol.fans.ToString();
        slot.dance.text = zoneType == DropZoneType.Dance ? $"{idol.dance + data.withTeacherBenefit}▲" : idol.dance.ToString();
        slot.vocal.text = zoneType == DropZoneType.Vocal ? $"{idol.vocal + data.withTeacherBenefit}▲" : idol.vocal.ToString();
        slot.visual.text = zoneType == DropZoneType.Visual ? $"{idol.visual + data.withTeacherBenefit}▲" : idol.visual.ToString();
        foreach (var buff in slot.buffList)
        {
            buff.SetActive(true); // 顯示加成效果物件 => 功能待增加
        }
    }

    public void ClearSlot(DropZoneType zoneType, int slotIndex)
    {
        var slotList = zoneType == DropZoneType.Member ? memberSlots : traineeSlots;
        var slot = slotList[slotIndex];
        slot.currentIdol = null;

        // 清掉文字
        slot.fans.text = "";
        slot.dance.text = "";
        slot.vocal.text = "";
        slot.visual.text = "";
        foreach (var buff in slot.buffList)
        {
            buff.SetActive(false); // 隱藏加成效果物件
        }
    }
}
