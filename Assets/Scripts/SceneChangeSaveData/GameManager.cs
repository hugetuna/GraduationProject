using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ResolutionManager ResolutionManager;
    public GameObject Canvas_Config;
    [Header("永久儲存資料")]
    public List<SoilSaveData> soilDataList = new List<SoilSaveData>();
    public List<IdolSaveData> idolDataList = new List<IdolSaveData>();
    public DaySaveData DayData;
    public string sceneNameSave = "";
    public ResourceSaveData ResourceData;
    public ChatSaveData chatSaveData;

    [Header("臨時儲存資料")]
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
            LoadGameConfigFromFile();
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
                currentClothIndex = idol.currentClothIndex,
                BHaveSetUp = true,
                basicTrainRecord = idol.basicTrainRecord,
                trainRecord = idol.trainRecord,
                equippedItemName = idol.equipmentItemNow != null ? idol.equipmentItemNow.itemName : ""
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
    [ContextMenu("Save Config")]
    public void SaveGameConfigToFile()
    {
        GameConfigDataWrapper configData = new GameConfigDataWrapper
        {
            // 音量設定
            masterVolume = AudioManager.Instance.volume,
            musicVolume = AudioManager.Instance.musicVolume,
            sfxVolume = AudioManager.Instance.sfxVolume,
            // 畫面設定
            resolutionWidth = ResolutionManager.targetWidth,
            resolutionHeight = ResolutionManager.targetHeight,
            fullScreenMode = ResolutionManager.targetFullScreenMode
        };
        string json = JsonUtility.ToJson(configData, true);
        string configPath = Path.Combine(Application.persistentDataPath, "gameconfig.json");
        File.WriteAllText(configPath, json);
        Debug.Log($"遊戲設定已儲存至：{configPath}");
    }
    [ContextMenu("Load Config")]
    public void LoadGameConfigFromFile()
    {
        string configPath = Path.Combine(Application.persistentDataPath, "gameconfig.json");
        if (!File.Exists(configPath))
        {
            Debug.LogWarning("找不到遊戲設定檔案。");
            return;
        }
        string json = File.ReadAllText(configPath);
        GameConfigDataWrapper configData = JsonUtility.FromJson<GameConfigDataWrapper>(json);
        //設置音效設定
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager 實例不存在，無法應用音量設定。");
            return;
        }
        // 應用音量設定
        AudioManager.Instance.SetVolume(configData.masterVolume);
        AudioManager.Instance.SetMusicVolume(configData.musicVolume);
        AudioManager.Instance.SetSFXVolume(configData.sfxVolume);
        // 應用畫面設定
        ResolutionManager.targetWidth = configData.resolutionWidth;
        ResolutionManager.targetHeight = configData.resolutionHeight;
        ResolutionManager.targetFullScreenMode = configData.fullScreenMode;
        Debug.Log("遊戲設定已載入。");
    }
    //關閉遊戲
    public void QuitGame()
    {
        Debug.Log("正在要求關閉遊戲...");

        // 1. 如果是在 Unity 編輯器內執行
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 停止按下 Play 的狀態
#else
            // 2. 如果是正式打包出來的程式
            Application.Quit(); 
#endif
    }
}
