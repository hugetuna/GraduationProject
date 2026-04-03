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
    //裝備
    public string equippedItemName;
    //當前衣服編號
    public int currentClothIndex;
    //使否初始化過
    public bool BHaveSetUp;

    public BasicTrainRecord basicTrainRecord; // 訓練記錄初始值
    public TrainRecord trainRecord; // 訓練紀錄
    public BaitoRecord baitoRecord; // 打工紀錄
    public ActivityRecord activityRecord; // 商演紀錄

    public bool isAvailable; // 是否可用（在隊伍中；在場景中啟用）
}
