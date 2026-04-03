using UnityEngine;

[System.Serializable] // 確保可以被序列化（存檔）
public class BaitoRecord
{
    public Baito selectedBaito; // 打工資訊（從這裡就能知道角色有沒有被指派打工）
    public Vector2 position; // 角色圖片在打工 UI 的位置
    public BaitoDropZoneType zoneType;
    public int zoneIndex;

    public void SetBaitoRecord(Baito selectedBaito,
                               Vector2? position = null,
                               BaitoDropZoneType? zoneType = null,
                               int? zoneIndex = null)
    {
        this.selectedBaito = selectedBaito;
        if (position != null) this.position = position.Value;
        if (zoneType != null) this.zoneType = zoneType.Value;
        if (zoneIndex != null) this.zoneIndex = zoneIndex.Value;
    }

}
