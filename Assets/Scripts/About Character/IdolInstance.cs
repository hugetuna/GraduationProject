using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IdolWho {none=-1,Kuma=0,Karo=1,Sirius=2}
public class IdolInstance : MonoBehaviour
{
    public IdolWho idolIndex;
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
    // Start is called before the first frame update
    void Start()
    {
        if (BHaveSetUp == false)
        {
            IdolSetUp();
        }
        
    }
    //初始化
    private void IdolSetUp()
    {
        if (basicStatus == null)
        {
            Debug.LogError("基本狀態 (basicStatus) 未設定！");
            return;
        }
        vocal = basicStatus.vocal;
        dance = basicStatus.dance;
        visual = basicStatus.visual;
        voTrainingBonus=1f;
        daTrainingBonus=1f;
        viTrainingBonus=1f;
        charm = basicStatus.charm;
        charmInCount = charm;
        performance = basicStatus.performance;
        vigour = vigourMax = basicStatus.vigour;
        fans = 0;
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
        charm=data.charm;//基礎魅力
        charmInCount=data.charmInCount;//魅力->暫時上升或下降後的數值，結算也用這個
        performance=data.performance;//演技
        vigour=data.vigour;//體力
        vigourMax=data.vigourMax;//體力最大值
        //粉絲數
        fans=data.fans;
        bondWithP=data.bondWithP;//與玩家的羈絆
        BHaveSetUp = data.BHaveSetUp;
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
}
