using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.U2D.Animation;

public enum IdolWho { none = -1, Kuma = 0, Karo = 1, Sirius = 2 ,Mizar=3,Aicor=4}
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
    //衣服編號->string字典
    public Dictionary<int, string> clothesDict = new Dictionary<int, string>();
    public int currentClothIndex = 0;//目前穿著的衣服編號
    //道具相關
    public int plantVigourCost = 5;
    public int waterVigourCost=3;
    //是否已經完成初始化
    public bool BHaveSetUp = false;

    public Sprite spriteQ; // 角色 UI 圖片（Q 版）
    public Sprite spriteTachie; // 角色 UI 圖片（立繪）
    public BasicTrainRecord basicTrainRecord; // 初始值存放地
    public TrainRecord trainRecord = new(); // 訓練紀錄

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

        if (basicTrainRecord == null)
        {
            Debug.LogError("基本狀態 (basicTrainRecord) 未設定！");
            return;
        }
        trainRecord.SetTrainRecord(basicTrainRecord.state, 
                                   basicTrainRecord.position,
                                   basicTrainRecord.droppedZoneType,
                                   basicTrainRecord.droppedZoneIndex,
                                   basicTrainRecord.vigourCost,
                                   basicTrainRecord.danceExp,
                                   basicTrainRecord.vocalExp,
                                   basicTrainRecord.visualExp,
                                   basicTrainRecord.isActive);
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
        //衣服編號
        clothesDict = new Dictionary<int, string>()
        {
            {0,"normal" },
            {1,"clo1" },
            {2,"clo2" },
            {3,"clo3" },
        };
        currentClothIndex = data.currentClothIndex;
        ChangeCloth(currentClothIndex);

        // 訓練紀錄
        basicTrainRecord = data.basicTrainRecord;
        trainRecord = data.trainRecord;
                                  
        positionInTeam = data.positionInTeam;
    }
    public bool costVigour(int Amount)
    {
        if (vigour - Amount >= 0)
        {
            vigour -= Amount;
            UpdateMainStatusBar();
            return true;
        }
        else
        {
            return false;
        }
    }
    public void recoverVigour(int Amount)
    {
        vigour += Amount;
        if (vigour > vigourMax)
        {
            vigour = vigourMax;
        }
        UpdateMainStatusBar();
    }

    public void UpdateMainStatusBar()
    {
        MainCanvasSetter mainCanvasSetter = FindAnyObjectByType<MainCanvasSetter>();
        if (mainCanvasSetter != null)
        {
            mainCanvasSetter.setStatusBar();
        }
    }

    public void ChangeCloth(int ClothIndex)
    {
        //找到目標偶像的Transform
        Transform TargetTransform = transform.Find("KumaQ2.0");
        if (TargetTransform == null)
        {
            TargetTransform = transform.Find("KaroQ2.0");
        }
        if (TargetTransform == null)
        {
            TargetTransform = transform.Find("SiriusQ2.0");
        }
        //if (TargetTransform == null)
        //{
        //    TargetTransform = transform.Find("MizarQ2.0");
        //}
        //if (TargetTransform == null)
        //{
        //    TargetTransform = transform.Find("AicorQ2.0");
        //}
        if (TargetTransform == null)
        {
            return;
        }
        //取得各部位的SpriteResolver
        Dictionary<string, SpriteResolver> resolvers = new Dictionary<string, SpriteResolver>()
    {
        { "Body", TargetTransform.Find("Body").GetComponent<SpriteResolver>() },
        { "LHand", TargetTransform.Find("LHand").GetComponent<SpriteResolver>() },
        { "RHand", TargetTransform.Find("RHand").GetComponent<SpriteResolver>() },
        { "LLeg", TargetTransform.Find("LLeg").GetComponent<SpriteResolver>() },
        { "RLeg", TargetTransform.Find("RLeg").GetComponent<SpriteResolver>() },
        { "OnHead", TargetTransform.Find("OnHead").GetComponent<SpriteResolver>() },
    };
        //根據index更換衣服
        foreach (var resolver in resolvers)
        {
            resolver.Value.SetCategoryAndLabel(resolver.Key, clothesDict[ClothIndex]);
        }
        currentClothIndex = ClothIndex;
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
        MainCanvasSetter mainCanvasSetter = FindAnyObjectByType<MainCanvasSetter>();
        if (mainCanvasSetter != null)
        {
            mainCanvasSetter.setStatusBar();
            mainCanvasSetter.setStatusBoard();
        }
        Debug.Log($"{idolIndex}的粉絲數增加了{increseAmount}，目前粉絲數為{fans}");
    }

    // 每天結束時的訓練結算＆記錄重置
    public void SettleTrainRecord()
    {
        vigour -= trainRecord.vigourCost; // 隔天主 UI 會同步此變化
        dance += trainRecord.danceExp;
        vocal += trainRecord.vocalExp;
        visual += trainRecord.visualExp;

        // 重置必須清空的訓練紀錄（三種訓練數值）
        trainRecord.SetTrainRecord(IdolTrainingState.InTeam,
                                   Vector2.zero,
                                   DropZoneType.Member,
                                   -1,
                                   basicTrainRecord.vigourCost,
                                   basicTrainRecord.danceExp,
                                   basicTrainRecord.vocalExp,
                                   basicTrainRecord.visualExp,
                                   true);
    }
}
