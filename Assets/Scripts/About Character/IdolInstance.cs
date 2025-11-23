using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IdolWho { none = -1, Kuma = 0, Karo = 1, Sirius = 2 }
public class IdolInstance : MonoBehaviour
{
    public IdolWho idolIndex;
    public int positionInTeam;//站位編號
    //角色的基礎數值
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

    public bool BHaveSetUp = false;

    // 訓練紀錄
    public BasicTrainRecord BasicTrainRecord; // 初始值存放地
    public IdolTrainingState state; // 在隊伍或者特定訓練室
    public Vector2 positionInTrainingUI; // 代表圖片在訓練 UI 的位置
    public int vigourCost; // 體力消耗
    public int danceExp; // 舞蹈收益
    public int vocalExp; // 歌唱收益
    public int visualExp; // 表現力收益
    public bool isActive = true;//是否在場景中啟用

    // Start is called before the first frame update
    void Start()
    {
        if (BHaveSetUp == false)
        {
            IdolSetUp();
        }

    }
    //初始化
    public void IdolSetUp()
    {
        if (basicStatus == null)
        {
            Debug.LogError("基本狀態 (basicStatus) 未設定！");
            return;
        }
        vocal = basicStatus.vocal;
        dance = basicStatus.dance;
        visual = basicStatus.visual;
        voTrainingBonus = 1f;
        daTrainingBonus = 1f;
        viTrainingBonus = 1f;
        charm = basicStatus.charm;
        charmInCount = charm;
        performance = basicStatus.performance;
        vigour = vigourMax = basicStatus.vigour;
        fans = 0;

        state = BasicTrainRecord.state;
        positionInTrainingUI = BasicTrainRecord.position;
        vigourCost = BasicTrainRecord.vigourCost;
        danceExp = BasicTrainRecord.danceExp;
        vocalExp = BasicTrainRecord.vocalExp;
        visualExp = BasicTrainRecord.visualExp;
    }
    //填入讀取的資料組
    public void LoadData(IdolSaveData data)
    {
        idolIndex = data.idolIndex;
        basicStatus = data.basicStatus;
        vocal = data.vocal;
        dance = data.dance;
        visual = data.visual;
        voTrainingBonus = data.voTrainingBonus;
        daTrainingBonus = data.daTrainingBonus;
        viTrainingBonus = data.viTrainingBonus;
        charm = data.charm;//基礎魅力
        charmInCount = data.charmInCount;//魅力->暫時上升或下降後的數值，結算也用這個
        performance = data.performance;//演技
        vigour = data.vigour;//體力
        vigourMax = data.vigourMax;//體力最大值
        //粉絲數
        fans = data.fans;
        bondWithP = data.bondWithP;//與玩家的羈絆
        BHaveSetUp = data.BHaveSetUp;

        // 訓練紀錄
        state = data.state;
        positionInTrainingUI = data.positionInTrainingUI;
        vigourCost = data.vigourCost;
        danceExp = data.danceExp;
        vocalExp = data.vocalExp;
        visualExp = data.visualExp;
        BasicTrainRecord = data.basicTrainRecord;
        isActive = data.isActive;

        positionInTeam = data.positionInTeam;
    }
    public bool costVigour(int Amount)
    {
        if (vigour - Amount >= 0)
        {
            vigour -= Amount;
            MainCanvasSetter mainCanvasSetter = FindAnyObjectByType<MainCanvasSetter>();
            mainCanvasSetter.setStatusBar();
            return true;
        }
        else
        {
            return false;
        }
    }
    //每天結束時必須重製全員暫時狀態
    public void ResetTemporaryEffect()
    {
        charmInCount = charm;
        voTrainingBonus = 1f;
        daTrainingBonus = 1f;
        viTrainingBonus = 1f;
    }
    public void FansIncrease(int increseAmount)
    {
        fans += increseAmount;
    }

    // 每天結束時的訓練結算＆記錄重置
    public void SettleTrainRecord()
    {
        vigour -= vigourCost;
        dance += danceExp;
        vocal += vocalExp;
        visual += visualExp;

        // 重置訓練紀錄
        state = BasicTrainRecord.state;
        positionInTrainingUI = BasicTrainRecord.position;
        vigourCost = BasicTrainRecord.vigourCost;
        danceExp = BasicTrainRecord.danceExp;
        vocalExp = BasicTrainRecord.vocalExp;
        visualExp = BasicTrainRecord.visualExp;
        // isActive = true;
    }
}
