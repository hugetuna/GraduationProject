using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDataWrapper
{
    public List<SoilSaveData> soilDataList;
    public List<IdolSaveData> idolDataList;
    public DaySaveData DayData;
    public ResourceSaveData ResourceData;
    public ChatSaveData chatSaveData;
    public bool isElevatorUsedToday;
    // 劇情與舞台通常視需求決定是否永久存檔
    // DialogueSaveData dialogueSaveData;
}
