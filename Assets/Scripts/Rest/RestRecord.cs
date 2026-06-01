using UnityEngine;

[System.Serializable]
public class RestRecord
{
    public Vector2 position; // 角色圖片在休息室 UI 的位置
    public RestDropZoneType zoneType;
    public int zoneIndex;
    public int vigourEarned = 50; // 這次休息獲得的體力值

    public void SetRestRecord(Vector2? position = null,
                              RestDropZoneType? zoneType = null,
                              int? zoneIndex = null)
    {
        if (position != null) this.position = position.Value;
        if (zoneType != null) this.zoneType = zoneType.Value;
        if (zoneIndex != null) this.zoneIndex = zoneIndex.Value;
    }
}
