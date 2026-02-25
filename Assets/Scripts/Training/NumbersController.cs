using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class StatsSlot
{
    public TextMeshProUGUI fans;
    public TextMeshProUGUI dance;
    public TextMeshProUGUI vocal;
    public TextMeshProUGUI visual;
    public Image equipmentIcon = null;
    public List<GameObject> buffList; // 顯示加成效果的物件列表（底下的文字和圖示另外設定）
    public IdolInstance currentIdol; // 這個 slot 目前放哪個角色
}

/* 控制角色拖曳後的數值顯示，掛在不同的訓練 UI 上 */
public class NumbersController : MonoBehaviour
{
    [SerializeField] private List<StatsSlot> memberSlots = new();
    [SerializeField] private List<StatsSlot> traineeSlots = new();
    [SerializeField] private TrainingType trainingType = TrainingType.None;
    public static event Action<IdolWho, DropZoneType, int, TrainingUIData> OnIdolPositionChanged;

    void Start()
    {
        OnIdolPositionChanged += HandleIdolPositionChanged; // 訂閱事件
    }

    void OnDestroy()
    {
        OnIdolPositionChanged -= HandleIdolPositionChanged; // 取消訂閱事件
    }

    public void InitializeSlots(TrainingUIData data)
    {
        ClearAllSlots(); // 先全部清空

        // 分區處理
        var idols = TeamDataUtility.IdolInstanceList;
        var zoneGroups = idols.GroupBy(idol => idol.trainRecord.droppedZoneType);

        foreach (var group in zoneGroups)
        {
            // 找出該區目前「原本」最大的 Index，當作衝突時往後遞補的基準
            // 如果該區是空的，max 給 -1，這樣下面 +1 就會從 0 開始
            int currentMaxIndex = group.Any() ? group.Max(x => x.trainRecord.droppedZoneIndex) : -1;

            // 用來記錄「已經發出去的號碼牌」
            HashSet<int> occupiedIndices = new();

            // 排序：先照 Index 排，如果 Index 一樣就照 ID 排 (保證每次執行結果順序固定)
            var sortedIdols = group
                .OrderBy(idol => idol.trainRecord.droppedZoneIndex)
                .ThenBy(idol => idol.idolIndex)
                .ToList();

            // 分配角色 UI 位置
            foreach (var idol in sortedIdols)
            {
                int finalIndex = idol.trainRecord.droppedZoneIndex;

                // 檢查：如果這個座位已經有人坐了 (發生衝突)
                if (occupiedIndices.Contains(finalIndex))
                {
                    // 直接把這個人丟到「當前最大值 + 1」的位置
                    currentMaxIndex++;
                    finalIndex = currentMaxIndex;

                    // 同步到跨場景資料
                    TraineeAssignment.UpdateTrainRecord(idol.idolIndex, droppedZoneIndex: finalIndex);
                }
                // 如果 currentMaxIndex 比現在的 index 小記得更新
                else if (finalIndex > currentMaxIndex)
                {
                    currentMaxIndex = finalIndex;
                }

                // 登記座位，防止下一個人又搶到這個位置
                occupiedIndices.Add(finalIndex);

                // 根據位置資料更新 UI
                HandleIdolPositionChanged(idol.idolIndex, group.Key, finalIndex, data);
            }
        }
    }

    public void RefreshSlots(TrainingUIData data)
    {
        // 刷新成員區
        foreach (var slot in memberSlots)
        {
            if (slot.currentIdol != null)
            {
                FillSlotData(slot, slot.currentIdol.idolIndex, DropZoneType.Member, data);
            }
        }

        // 刷新訓練區
        foreach (var slot in traineeSlots)
        {
            if (slot.currentIdol != null)
            {
                // DropZoneType：Dance、Vocal、Visual = 1、2、3
                // TrainingType：Dance、Vocal、Visual = 0、1、2
                FillSlotData(slot, slot.currentIdol.idolIndex, (DropZoneType)(trainingType + 1), data);
            }
        }
    }

    private void HandleIdolPositionChanged(IdolWho idolIndex, DropZoneType zoneType, int slotIndex, TrainingUIData data)
    {
        // 不論該角色被拖曳到哪，先檢查該 UI 內部是否正在顯示該角色，有就清掉（解決殘影問題）
        RemoveIdolFromPreSlots(idolIndex);

        // 判斷角色的「新位置」是否屬於此 UI 的管轄範圍
        string targetZoneTypeStr = zoneType.ToString();

        // 1. 如果去的是成員區 (Member)，所有 UI 的成員區都要更新。
        // 2. 如果去的是訓練區，則只有「類型符合」的 UI 才要更新訓練區。
        bool isMyBusiness = (zoneType == DropZoneType.Member) || (targetZoneTypeStr == trainingType.ToString());
        if (!isMyBusiness) return;

        // 將角色數值填到對應的 slot
        var slotList = (zoneType == DropZoneType.Member) ? memberSlots : traineeSlots;

        if (slotIndex >= 0 && slotIndex < slotList.Count)
        {
            FillSlotData(slotList[slotIndex], idolIndex, zoneType, data);
        }
    }

    private void RemoveIdolFromPreSlots(IdolWho idolIndex)
    {
        // 檢查並清除成員區
        for (int i = 0; i < memberSlots.Count; i++)
        {
            if (memberSlots[i].currentIdol != null && memberSlots[i].currentIdol.idolIndex == idolIndex)
            {
                ClearSlotUI(memberSlots[i]);
            }
        }
        // 檢查並清除訓練區
        for (int i = 0; i < traineeSlots.Count; i++)
        {
            if (traineeSlots[i].currentIdol != null && traineeSlots[i].currentIdol.idolIndex == idolIndex)
            {
                ClearSlotUI(traineeSlots[i]);
            }
        }
    }

    private void FillSlotData(StatsSlot slot, IdolWho idolIndex, DropZoneType zoneType, TrainingUIData data)
    {
        var idol = TeamDataUtility.IdolDict[idolIndex];
        slot.currentIdol = idol;

        // 更新數值文字＆裝備圖示
        if (slot.equipmentIcon != null)
        {
            if (idol.equipmentItemNow != null) slot.equipmentIcon.sprite = idol.equipmentItemNow.icon;
            else slot.equipmentIcon.sprite = null;
        }
        slot.fans.text = idol.fans.ToString();

        var teacherName = GameManager.Instance.teacherSaveData.GetTeacherNameByType(data.trainingType);
        int benefit = (teacherName != "無") ? data.withTeacherBenefit : data.basicBenefit;
        slot.dance.text = (zoneType == DropZoneType.Dance) ? $"{idol.dance + (int)(benefit * idol.daTrainingBonus)}▲" : idol.dance.ToString();
        slot.vocal.text = (zoneType == DropZoneType.Vocal) ? $"{idol.vocal + (int)(benefit * idol.voTrainingBonus)}▲" : idol.vocal.ToString();
        slot.visual.text = (zoneType == DropZoneType.Visual) ? $"{idol.visual + (int)(benefit * idol.viTrainingBonus)}▲" : idol.visual.ToString();

        // 取得該角色目前有的訓練加成＆顯示加成物件
        var buffNames = ItemEffectUtility.GetTrainingEffectDisplayNames(idolIndex, data.trainingType);
        for (int i = 0; i < slot.buffList.Count; i++) // 以 buffBar 物件的數量為上限
        {
            if (i < buffNames.Count)
            {
                slot.buffList[i].SetActive(true);
                var buffBar = slot.buffList[i].GetComponent<BuffBar>();
                buffBar.UpdateBuffBar(buffNames[i]); // 更新 buffBar 上的文字（如果有圖示的話可以一起更新）
            }
            else
            {
                slot.buffList[i].SetActive(false); // 多出來的隱藏起來
            }
        }
    }

    private void ClearSlotUI(StatsSlot slot)
    {
        slot.currentIdol = null;
        if (slot.equipmentIcon != null) slot.equipmentIcon.sprite = null;
        slot.fans.text = "";
        slot.dance.text = "";
        slot.vocal.text = "";
        slot.visual.text = "";
        foreach (var buff in slot.buffList)
        {
            buff.SetActive(false);
        }
    }

    private void ClearAllSlots()
    {
        foreach (var slot in memberSlots) ClearSlotUI(slot);
        foreach (var slot in traineeSlots) ClearSlotUI(slot);
    }

    // 外部呼叫用
    public static void NotifyIdolMoved(IdolWho idolIndex, DropZoneType zoneType, int slotIndex, TrainingUIData data)
    {
        OnIdolPositionChanged?.Invoke(idolIndex, zoneType, slotIndex, data);
    }
}
