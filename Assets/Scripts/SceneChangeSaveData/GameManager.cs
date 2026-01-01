using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //永久儲存資料
    public List<SoilSaveData> soilDataList = new List<SoilSaveData>();
    public List<IdolSaveData> idolDataList = new List<IdolSaveData>();
    public DaySaveData DayData;
    public string sceneNameSave = "";
    public ResourceSaveData ResourceData;
    public ChatSaveData chatSaveData;
    //臨時儲存資料
    public DialogueSaveData dialogueSaveData;
    public StageAttribute onStageStage;
    public bool isElevatorUsedToday = false;

    //單例物件生成
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //土地專用儲存
    public void SaveSoilData(List<Soil> soils)
    {
        soilDataList.Clear();
        foreach (var soil in soils)
        {
            var data = new SoilSaveData
            {
                position = soil.transform.position,
                isPlantable = soil.isPlantable,
                isPlanting = soil.isPlanting
            };
            if (soil.seedOnThisSoil != null)
            {
                data.plantedSeedName = soil.seedOnThisSoil.seedData.seedName;
                data.isWatered = soil.seedOnThisSoil.GetIsWateredToday();
                data.daysGrown = soil.seedOnThisSoil.GetDaysGrown();
                data.currentRewardPoint = soil.seedOnThisSoil.GetRewardPoint();
            }
            soilDataList.Add(data);
        }
    }
    //偶像專用儲存
    public void SaveIdolData(List<IdolInstance> idols)
    {
        idolDataList.Clear();
        foreach (var idol in idols)
        {
            var data = new IdolSaveData
            {
                positionInTeam = idol.positionInTeam,
                basicStatus = idol.basicStatus,
                idolIndex = idol.idolIndex,
                vocal = idol.vocal,
                dance = idol.dance,
                visual = idol.visual,
                voTrainingBonus = idol.voTrainingBonus,
                daTrainingBonus = idol.daTrainingBonus,
                viTrainingBonus = idol.viTrainingBonus,
                charm = idol.charm,
                charmInCount = idol.charmInCount,
                performance = idol.performance,
                vigour = idol.vigour,
                vigourMax = idol.vigourMax,
                fans = idol.fans,
                bondWithP = idol.bondWithP,
                BHaveSetUp = true,
                basicTrainRecord = idol.basicTrainRecord,
                trainRecord = idol.trainRecord
            };
            idolDataList.Add(data);
        }
    }
    public void SaveDayData()
    {
        DayData.day = DayManager.Instance.date;
        DayData.currentEventIndex = DayManager.Instance.dayEventManager.currentEvent.TriggerTimeIndex;
        DayData.IsInStartOfDay = DayManager.Instance.IsInStartOfDay;
    }
    //保存劇情文本(方便轉場時直接使用)
    public void SaveInkJSONAssetData(DialogueSaveData data)
    {
        dialogueSaveData = data;
    }
    //保存舞台資料(方便轉場時直接使用)
    public void SaveStageAttribute(StageAttribute data)
    {
        onStageStage = data;
    }
    //資源專用儲存
    public void SaveResourceData(ResourceManager resource)
    {
        //ResourceData = null;
        ResourceData.Money = resource.getMoney();
        ResourceData.MoneyBonus = resource.MoneyBonus;
        ResourceData.bondAB = resource.bondAB;
        ResourceData.bondBC = resource.bondBC;
        ResourceData.bondCA = resource.bondCA;
        ResourceData.items = resource.items;
    }

    //聊天視窗專用儲存
    public void SaveChatData(ChatSaveData data)
    {
        chatSaveData = data;
    }

    /*本地存檔相關*/
    private string SavePath => Path.Combine(Application.persistentDataPath, "gamesave.json");

    [ContextMenu("Save Game")]
    public void SaveToFile()
    {
        // 1. 準備包裝好的資料
        SaveDataWrapper wrapper = new SaveDataWrapper
        {
            soilDataList = this.soilDataList,
            idolDataList = this.idolDataList,
            ResourceData = this.ResourceData,
            DayData = this.DayData,
            sceneNameSave = this.sceneNameSave,
            chatSaveData = this.chatSaveData,
            isElevatorUsedToday = this.isElevatorUsedToday,
            //dialogueSaveData = this.dialogueSaveData
        };

        // 2. 轉化為 JSON 字串 (true 代表要格式化字串，方便人類閱讀)
        string json = JsonUtility.ToJson(wrapper, true);

        // 3. 寫入檔案
        File.WriteAllText(SavePath, json);
        Debug.Log($"存檔成功！路徑：{SavePath}");
    }

    [ContextMenu("Load Game")]
    public void LoadFromFile()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("找不到存檔檔案。");
            return;
        }

        // 1. 讀取字串
        string json = File.ReadAllText(SavePath);

        // 2. 解析 JSON
        SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);

        // 3. 還原到 GameManager
        this.soilDataList = wrapper.soilDataList;
        this.idolDataList = wrapper.idolDataList;
        this.ResourceData = wrapper.ResourceData;
        this.DayData = wrapper.DayData;
        this.sceneNameSave = wrapper.sceneNameSave;
        this.chatSaveData = wrapper.chatSaveData;
        this.isElevatorUsedToday = wrapper.isElevatorUsedToday;
        //this.dialogueSaveData = wrapper.dialogueSaveData;

        //4.處理讀取後的資料
        DayManager.Instance.OnGameFileLoad();
        SceneTransitionManager.Instance.OnGameFileLoad();

        Debug.Log("存檔已載入。");
    }
}
