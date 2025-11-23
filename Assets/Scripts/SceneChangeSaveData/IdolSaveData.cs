using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class IdolSaveData
{
    // 站位編號
    public int positionInTeam;
    //標示身分
    public IdolWho idolIndex;
    public Idols basicStatus;
    //實際運行的數值
    public int vocal;//歌唱力
    public int dance;//舞蹈力
    public int visual;//表現力
    //三種練習效率
    public float voTrainingBonus;
    public float daTrainingBonus;
    public float viTrainingBonus;
    public int charm;//基礎魅力
    public int charmInCount;//魅力->暫時上升或下降後的數值，結算也用這個
    public int performance;//演技
    public int vigour;//體力
    public int vigourMax;//體力最大值
    //粉絲數
    public int fans;
    public int bondWithP;//與玩家的羈絆
    //使否初始化過
    public bool BHaveSetUp;

    // 訓練記錄
    public BasicTrainRecord basicTrainRecord; // 初始值存放地
    public IdolTrainingState state; // 在隊伍或者特定訓練室
    public Vector2 positionInTrainingUI; // 代表圖片在訓練 UI 的位置
    public int vigourCost; // 體力消耗
    public int danceExp; // 舞蹈收益
    public int vocalExp; // 歌唱收益
    public int visualExp; // 表現力收益
    public bool isActive;
}
