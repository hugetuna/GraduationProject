using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicTrainRecord", menuName = "Training/BasicTrainRecord")]

public class BasicTrainRecord : ScriptableObject
{
    public IdolTrainingState state = IdolTrainingState.InTeam; // 在隊伍或者特定訓練室
    public Vector2 position = Vector2.zero; // 代表圖片在訓練 UI 的位置
    public DropZoneType droppedZoneType = DropZoneType.Member; // 代表圖片所在的拖曳區域類型
    public int droppedZoneIndex = -1; // 代表圖片所在的拖曳區域索引（會在 PickManager 中直接設定給 TrainRecord）
    public int vigourCost = 0; // 體力消耗
    public int danceExp = 0; // 舞蹈收益
    public int vocalExp = 0; // 歌唱收益
    public int visualExp = 0; // 表現力收益
    // public bool isActive = true; //是否在場景中啟用
}
