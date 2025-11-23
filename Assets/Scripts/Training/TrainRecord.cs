using UnityEngine;


/* 用來儲存角色訓練成果（與 IdolInstance 連接，並透過 GameManager 跨場景保存資料） */
public class TrainRecord
{
    public IdolTrainingState state; // 在隊伍或者特定訓練室
    public Vector2 position; // 代表圖片在訓練 UI 的位置
    public int vigourCost; // 體力消耗
    public int danceExp; // 舞蹈收益
    public int vocalExp; // 歌唱收益
    public int visualExp; // 表現力收益
}
