using UnityEngine;


/* 用來儲存角色訓練成果（與 IdolInstance 連接，並透過 GameManager 跨場景保存資料） */
[System.Serializable]
public class TrainRecord
{
    public IdolTrainingState state; // 在隊伍或者特定訓練室
    public Vector2 position; // 代表圖片在訓練 UI 的位置
    public DropZoneType droppedZoneType; // 代表圖片所在的拖曳區域類型
    public int droppedZoneIndex; // 代表圖片所在的拖曳區域索引
    public int vigourCost; // 體力消耗
    public int danceExp; // 舞蹈收益
    public int vocalExp; // 歌唱收益
    public int visualExp; // 表現力收益
    // public bool isActive; //是否在場景中啟用

    public void SetTrainRecord(IdolTrainingState state = IdolTrainingState.None,
                               Vector2? position = null,
                               DropZoneType droppedZoneType = DropZoneType.None,
                               int droppedZoneIndex = -1,
                               int? vigourCost = null,
                               int? dance = null,
                               int? vocal = null,
                               int? visual = null
                               /*bool? isActive = null*/)
    {
        if (state != IdolTrainingState.None) this.state = state;
        if (position != null) this.position = position.Value;
        if (droppedZoneType != DropZoneType.None) this.droppedZoneType = droppedZoneType;
        if (droppedZoneIndex != -1) this.droppedZoneIndex = droppedZoneIndex;
        if (vigourCost != null) this.vigourCost = vigourCost.Value;
        if (dance != null) danceExp = dance.Value;
        if (vocal != null) vocalExp = vocal.Value;
        if (visual != null) visualExp = visual.Value;
        // if (isActive != null) this.isActive = isActive.Value;
    }

    public void RestrictTrainingOneDay()
    {
        // 在一天開始將訓練狀態設為 Unable，表示這一整天都無法訓練
        // 當天結束（結算時）IdolInstance 會自動重置成 IdolTrainingState.InTeam
        state = IdolTrainingState.Unable;
    }

    // 在隊伍裡的角色都算在可訓練範圍內（包含無法訓練的特殊狀態）
    public bool IsInTeamScope()
    {
        // if (state == IdolTrainingState.None) Debug.LogError("訓練紀錄的狀態未設定！");
        return state == IdolTrainingState.InTeam || state == IdolTrainingState.Unable;
    }
}
