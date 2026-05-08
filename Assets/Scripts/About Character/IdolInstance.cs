using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.U2D.Animation;

public enum IdolWho { none = -1, Kuma = 0, Karo = 1, Sirius = 2, Mizar = 3, Aicor = 4 }
public enum AvailableAction { Free, Train, Baito, Activity }
public class IdolInstance : MonoBehaviour
{
    public IdolWho idolIndex;
    public int positionInTeam;//站位編號
    //角色的基礎數值
    public Idols basicStatus;
    public List<Idols> basicStatusList = new List<Idols>();//用來存放基本數值的列表，讀取時會根據idolIndex選擇對應的基本數值
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
    public EquipmentItem equipmentItemNow = null;
    public string equippedItemName; //用來存裝備的名字(unity無法直接存OS)
    //衣服編號->string字典
    public Dictionary<int, string> clothesDict = new Dictionary<int, string>();
    public int currentClothIndex = 0;//目前穿著的衣服編號
    //道具相關
    public int plantVigourCost = 5;
    public int waterVigourCost = 3;
    //是否已經完成初始化
    public bool BHaveSetUp = false;

    public IdolUISprites idolUISprites; // 存放偶像 UI 圖像
    public BasicTrainRecord basicTrainRecord; // 初始值存放地
    public TrainRecord trainRecord = new(); // 訓練紀錄
    public BaitoRecord baitoRecord = new(); // 打工紀錄
    public ActivityRecord activityRecord = new(); // 商演紀錄
    public bool isAvailable; // 是否可用（在場景中啟用）
    public AvailableAction currentAction; // 當前行動狀態（空閒、訓練、打工、商演）


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
        isAvailable = true;
        currentAction = AvailableAction.Free;
        trainRecord.SetTrainRecord(
            basicTrainRecord.state,
            basicTrainRecord.position,
            basicTrainRecord.droppedZoneType,
            basicTrainRecord.droppedZoneIndex,
            basicTrainRecord.vigourCost,
            basicTrainRecord.danceExp,
            basicTrainRecord.vocalExp,
            basicTrainRecord.visualExp
        /* basicTrainRecord.isActive */
        );
        baitoRecord.SetBaitoRecord(
            null,
            Vector2.zero,
            BaitoDropZoneType.Member,
            -1
        );
        activityRecord.SetActivityRecord(null);
    }
    //填入讀取的資料組
    public void LoadData(IdolSaveData data)
    {
        idolIndex = data.idolIndex;
        switch (idolIndex)
        {
            case IdolWho.Kuma:
                basicStatus = basicStatusList[0];
                break;
            case IdolWho.Karo:
                basicStatus = basicStatusList[1];
                break;
            case IdolWho.Sirius:
                basicStatus = basicStatusList[2];
                break;
            case IdolWho.Mizar:
                basicStatus = basicStatusList[3];
                break;
            case IdolWho.Aicor:
                basicStatus = basicStatusList[4];
                break;
            default:
                Debug.LogError("讀取資料時，idolIndex不合法！");
                return;
        }
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
        //裝備
        equippedItemName = data.equippedItemName;
        if (equippedItemName != null && equippedItemName != "")
        {
            EquipmentItem equipmentBridg = ResourceManager.Instance.InventoryManager.FindEquipmentByName(equippedItemName);
            ResourceManager.Instance.InventoryManager.TryEquip(equipmentBridg, this);
        }
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
        // 打工紀錄
        baitoRecord = data.baitoRecord;
        // 商演紀錄
        activityRecord = data.activityRecord;

        isAvailable = data.isAvailable;
        currentAction = data.currentAction;

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
        if (TargetTransform == null)
        {
            TargetTransform = transform.Find("MizarQ2.0");
        }
        if (TargetTransform == null)
        {
            TargetTransform = transform.Find("AicorQ2.0");
        }
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

    // 判斷角色是否可以在特定的介面中顯示（訓練、打工、商演介面）
    public bool CanShowInTheAction(AvailableAction actionType)
    {
        // 1. 如果角色是閒置的，在哪個介面都可以顯示
        if (currentAction == AvailableAction.Free) return true;

        // 2. 如果角色正在忙，只有「對應的活動」可以顯示他
        return currentAction == actionType;
    }

    // 每天結束時的訓練結算＆記錄重置
    public void SettleRecords()
    {
        if (currentAction == AvailableAction.Activity)
        {
            SettleActivityRecord(true); // 如果有商演，以下兩種行動不會結算並直接重置
            SettleTrainRecord(false);
            SettleBaitoRecord(false);
        }
        else if (currentAction == AvailableAction.Train)
        {
            SettleTrainRecord(true);
            SettleActivityRecord(false);
            SettleBaitoRecord(false);
        }
        else if (currentAction == AvailableAction.Baito)
        {
            SettleBaitoRecord(true);
            SettleActivityRecord(false);
            SettleTrainRecord(false);
        }
        else // Free 狀態則不結算任何東西，直接重置
        {
            SettleBaitoRecord(false);
            SettleActivityRecord(false);
            SettleTrainRecord(false);
        }

    }

    public void SettleActivityRecord(bool isSettling)
    {
        if (isSettling && activityRecord.selectedActivity != null)
        {
            Debug.Log($"結算 {idolIndex} 的商演");
            vigour -= activityRecord.selectedActivity.vigourCost; // 隔天主 UI 會同步此變化
            ResourceManager.Instance.Money += activityRecord.selectedActivity.MoneyGain;
        }

        // 重置必須清空的商演紀錄
        activityRecord.SetActivityRecord(null);
    }

    public void SettleTrainRecord(bool isSettling)
    {
        if (isSettling)
        {
            Debug.Log($"結算 {idolIndex} 的訓練");
            vigour -= trainRecord.vigourCost; // 隔天主 UI 會同步此變化
            dance += trainRecord.danceExp;
            vocal += trainRecord.vocalExp;
            visual += trainRecord.visualExp;
        }

        // 重置必須清空的訓練紀錄（三種訓練數值）
        trainRecord.SetTrainRecord(
            basicTrainRecord.state,
            basicTrainRecord.position,
            basicTrainRecord.droppedZoneType,
            basicTrainRecord.droppedZoneIndex,
            basicTrainRecord.vigourCost,
            basicTrainRecord.danceExp,
            basicTrainRecord.vocalExp,
            basicTrainRecord.visualExp
        /* true */
        );
    }
    // 進入電腦頁面時打工收益進帳＆記錄重置
    public void SettleBaitoRecord(bool isSettling)
    {
        if (isSettling && baitoRecord.selectedBaito != null)
        {
            Debug.Log($"結算 {idolIndex} 的打工");
            vigour -= baitoRecord.selectedBaito.vigourCost; // 隔天主 UI 會同步此變化
            ResourceManager.Instance.Money += baitoRecord.selectedBaito.MoneyGain;
        }

        // 重置必須清空的打工紀錄
        baitoRecord.SetBaitoRecord(
            null,
            Vector2.zero,
            BaitoDropZoneType.Member,
            -1
        );
    }
    [ContextMenu("TestEquip")]
    public void TestEquip()
    {
        EquipmentItem equipmentBridg = ResourceManager.Instance.InventoryManager.FindEquipmentByName("4分音符髮飾");
        ResourceManager.Instance.InventoryManager.TryEquip(equipmentBridg, this);
    }
}
