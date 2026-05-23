using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
public enum IdolTeamIndex
{
    None=-1,
    Kuma_Sirius_Karo=0,
    Kuma_Sirius_Aicor=1,
    Kuma_Sirius_Mizar=2,
    Kuma_Karo_Aicor=3,
    Kuma_Karo_Mizar=4,
    Kuma_Aicor_Mizar=5,
    Sirius_Karo_Aicor=6,
    Sirius_Karo_Mizar=7,
    Sirius_Aicor_Mizar=8,
    Karo_Aicor_Mizar= 9
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ResolutionManager ResolutionManager;
    public GameObject Canvas_Config;
    public GameSettingsSO startGameSettingsSO;
    [Header("永久儲存資料")]
    public List<AnimalFarmSaveData> FarmsDataList;
    public List<AnimalSaveData> animalDataList = new List<AnimalSaveData>();
    public int teamIndex = (int)IdolTeamIndex.None;
    public List<IdolSaveData> idolDataList = new List<IdolSaveData>();
    public DaySaveData DayData;
    public string sceneNameSave = "";
    public ResourceSaveData ResourceData;
    public ChatSaveData chatSaveData = new();
    public TeacherSaveData teacherSaveData = new();
    public ProductSaveData productSaveData = new();
    public ActivitySaveData activitySaveData = new();
    public DemonDialogueSaveData demonDialogueSaveData = new();

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
    public void SaveSoilData(List<AnimalFarm> Farms)
    {
        FarmsDataList.Clear();
        animalDataList.Clear();
        foreach (var Farm in Farms)
        {
            var farmData = new AnimalFarmSaveData
            {
                farmLV = Farm.farmLV,
                isActivated=Farm.isActivated,
                maxSeedAmount=Farm.maxSeedAmount,
                foodBarn=Farm.foodBarn,
                foodBarnMax=Farm.foodBarnMax
            };
            FarmsDataList.Add(farmData);
            foreach (var animal in Farm.seedsOnThisSoil)
            {
                var data = new AnimalSaveData
                {
                    farmLV=Farm.farmLV,
                    isWatered=animal.GetIsWateredToday(),
                    plantedSeedName=animal.seedData.seedName,
                    daysGrown=animal.GetDaysGrown(),
                    currentRewardPoint=animal.GetRewardPoint(),
                    RewardPointPerdrop=animal.seedData.wateredMinus
                };
                animalDataList.Add(data);
            }
        }
    }
    [ContextMenu("end a day test")]
    //意識到換天時場景不會在農場，所以需要一個方法專門在換天時更新農場和植物的資料
    public void UpdateFarmAndAnimalAtDayEnd()
    {
        foreach (var Farm in FarmsDataList)
        {
            foreach (var animal in animalDataList)
            {
                //只更新同一塊土地上的動物、每個動物消耗一次水分、成長一天、更新獎勵點數
                if (Farm.farmLV == animal.farmLV)
                {
                    //根據飼料儲量餵食
                    if (Farm.foodBarn > 0&&animal.isWatered == false)
                    {
                        animal.isWatered = true;
                        Farm.foodBarn -= 1;
                    }
                    else
                    {
                        animal.isWatered = false;
                        Farm.foodBarn = 0;
                    }
                    //成長一天，如果有被澆水則獎勵點數不下降
                    animal.daysGrown += 1;
                    if (animal.isWatered==false)
                    {
                        animal.currentRewardPoint -= animal.RewardPointPerdrop;
                    }
                    animal.isWatered = false; //每天結束時都要把水分狀態重置，因為每天都要重新澆水
                }
            }
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
                baitoRecord = idol.baitoRecord,
                activityRecord = idol.activityRecord,
                isAvailable = idol.isAvailable,
                currentAction = idol.currentAction,
                equippedItemName = idol.equipmentItemNow != null ? idol.equipmentItemNow.itemName : ""
            };
            idolDataList.Add(data);
        }
    }
    public void SaveDayData()
    {
        DayData.chapter=DayManager.Instance.chapter;
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
        //儲存道具ID和數量
        ResourceData.saveItems.Clear();
        foreach (ItemStack itemStack in resource.items)
        {
            bool isFansItem = itemStack.item is FansItem;
            IdolWho owner = IdolWho.none;
            if (itemStack.item is FansItem fansItem)
            {
                // 現在你可以直接存取 FansItem 特有的欄位了
                owner = fansItem.harvester;
            }
            ResourceData.saveItems.Add(new ItemSaveStack
            {
                itemID = itemStack.item.itemID,
                quantity = itemStack.quantity,
                isFansItem = isFansItem,
                Harvester = owner
            });
        }
        //塞入裝備名稱（因為裝備是以物件形式存在，無法直接存入，所以只存名字，讀取時再對照名字給裝備）
        ResourceData.allEqupmentNames.Clear();
        foreach (EquipmentItem equipment in resource.InventoryManager.ownedEquipments)
        {
            ResourceData.allEqupmentNames.Add(equipment.itemName);
        }
    }

    //聊天視窗專用儲存
    public void SaveChatData(UserRuntime user)
    {
        chatSaveData.users.Add(user);
    }

    // 預約相關儲存
    public void SaveTeacherData(TeacherInfo teacherInfo)
    {
        teacherSaveData.trainingTeachers.Add(teacherInfo);
    }

    // 商店商品儲存
    public void SaveProductData(ProductRuntime productRuntime)
    {
        productSaveData.products.Add(productRuntime);
    }

    public void SaveActivityData(Activity activity, int appointDay)
    {
        activitySaveData.activities.Add(new ActivityAppointment
        {
            activity = activity,
            appointDay = appointDay
        });
    }

    public void SaveDemonDialogueData(bool talkedStatus)
    {
        demonDialogueSaveData.hasTalkedToDemon = talkedStatus;
    }

    /*本地存檔相關*/
    private string SavePath => Path.Combine(Application.persistentDataPath, "gamesave.json");

    [ContextMenu("Save Game")]
    public void SaveToFile()
    {
        // 1. 準備包裝好的資料
        SaveDataWrapper wrapper = new SaveDataWrapper
        {
            animalDataList = this.animalDataList,
            FarmsDataList = this.FarmsDataList,
            teamIndex = this.teamIndex,
            idolDataList = this.idolDataList,
            ResourceData = this.ResourceData,
            DayData = this.DayData,
            sceneNameSave = this.sceneNameSave,
            chatSaveData = this.chatSaveData,
            teacherSaveData = this.teacherSaveData,
            productSaveData = this.productSaveData,
            activitySaveData = this.activitySaveData,
            demonDialogueSaveData = this.demonDialogueSaveData,
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
        this.animalDataList = wrapper.animalDataList;
        this.FarmsDataList = wrapper.FarmsDataList;
        this.teamIndex = wrapper.teamIndex;
        this.idolDataList = wrapper.idolDataList;
        this.ResourceData = wrapper.ResourceData;
        this.DayData = wrapper.DayData;
        this.sceneNameSave = wrapper.sceneNameSave;
        this.chatSaveData = wrapper.chatSaveData;
        this.teacherSaveData = wrapper.teacherSaveData;
        this.productSaveData = wrapper.productSaveData;
        this.activitySaveData = wrapper.activitySaveData;
        this.demonDialogueSaveData = wrapper.demonDialogueSaveData;
        this.isElevatorUsedToday = wrapper.isElevatorUsedToday;
        //this.dialogueSaveData = wrapper.dialogueSaveData;

        //4.處理讀取後的資料
        DayManager.Instance.OnGameFileLoad();
        SceneTransitionManager.Instance.OnGameFileLoad();
        ResourceManager.Instance.OnGameFileLoad();

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
    // 完全重置遊戲（回到初始狀態）並將 gamemanager 裡的資料也重置
    public void ResetGame()
    {
        if (startGameSettingsSO == null)
        {
            Debug.LogError("找不到初始設定檔 startGameSettingsSO！");
            return;
        }

        Debug.Log("正在透過 JSON 深拷貝重置遊戲資訊...");

        // 1. 將 SO 轉化為臨時 JSON 字串
        //string json = JsonUtility.ToJson(startGameSettingsSO);

        // 2. 利用 JsonUtility 解析出全新、獨立記憶體空間的資料物件
        //    這裡假設你有寫一個跟 SO 結構一樣的資料包裝類，或者我們直接轉回全新的實例
        //    最安全且符合你現有結構的做法，是逐個切斷 List 和自訂 Class 的引用：

        FarmsDataList = new List<AnimalFarmSaveData>(startGameSettingsSO.FarmsDataList);
        animalDataList = new List<AnimalSaveData>(startGameSettingsSO.animalDataList);
        idolDataList = new List<IdolSaveData>(startGameSettingsSO.idolDataList);

        teamIndex = startGameSettingsSO.teamIndex;
        sceneNameSave = startGameSettingsSO.sceneNameSave;

        // 以下是自訂的 Class 類別，必須用 JSON 轉化，否則會共用記憶體
        DayData = JsonUtility.FromJson<DaySaveData>(JsonUtility.ToJson(startGameSettingsSO.DayData));
        ResourceData = JsonUtility.FromJson<ResourceSaveData>(JsonUtility.ToJson(startGameSettingsSO.ResourceData));
        chatSaveData = JsonUtility.FromJson<ChatSaveData>(JsonUtility.ToJson(startGameSettingsSO.chatSaveData));
        teacherSaveData = JsonUtility.FromJson<TeacherSaveData>(JsonUtility.ToJson(startGameSettingsSO.teacherSaveData));
        productSaveData = JsonUtility.FromJson<ProductSaveData>(JsonUtility.ToJson(startGameSettingsSO.productSaveData));
        activitySaveData = JsonUtility.FromJson<ActivitySaveData>(JsonUtility.ToJson(startGameSettingsSO.activitySaveData));

        Debug.Log("遊戲資訊已成功安全重置，且初始設定檔未受污染。");
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
