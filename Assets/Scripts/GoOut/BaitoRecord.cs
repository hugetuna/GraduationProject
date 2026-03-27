using UnityEngine;

[System.Serializable]
public class BaitoRecord
{
    public Baito selectedBaito; // 打工資訊
    public Vector2 position; // 角色圖片在打工 UI 的位置
    public BaitoDropZoneType zoneType;
    public int zoneIndex;

    public bool isWorking; // 是否已被指派打工

    public void SetBaitoRecord(Baito selectedBaito = null,
                               Vector2? position = null,
                               BaitoDropZoneType? zoneType = null,
                               int? zoneIndex = null,
                               bool? isWorking = null)
    {
        if (selectedBaito != null) this.selectedBaito = selectedBaito;
        if (zoneType != null) this.zoneType = zoneType.Value;
        if (zoneIndex != null) this.zoneIndex = zoneIndex.Value;
        if (position != null) this.position = position.Value;
        if (isWorking != null) this.isWorking = isWorking.Value;
    }

}
