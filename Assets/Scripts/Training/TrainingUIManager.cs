using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum IdolTrainingState
{
    None = -1,
    InTeam = 0,
    InDance = 1,
    InVocal = 2,
    InVisual = 3,
    Unable = 4 // 在隊伍裡，但無法訓練的特殊狀態
}


/* 掛在 TrainingManager 上，統一管理三種不同的 UI（Singleton） */
public class TrainingUIManager : MonoBehaviour
{
    public static TrainingUIManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [SerializeField] private List<TrainingUIHandler> trainingUIHandlers = new();
    private TrainingUIHandler current = null; // 目前開著的 UI
    //-----------------------------------------------------------------//
    private Dictionary<IdolWho, IdolTrainingState> characterStates = new(); // 角色名稱＆訓練狀態對應表
    private bool isInitialized = false;


    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    void Start()
    {
        // 訂閱訓練 UI 的開關事件
        DoorInteraction.OnDoorInteracted += OpenOneUI;
        TrainingUIHandler.OnTrainingUIClosed += OneOneUIClosed;
    }

    public void InitializeTeamData()
    {
        characterStates.Clear();

        // 設定角色的初始訓練狀態＆圖片（也考慮了跨場景的情形）
        foreach (var dict in TeamDataUtility.IdolDict)
        {
            if (dict.Value == null || dict.Key == IdolWho.none) continue;

            // TeamDataUtility 內的資料本身會自動更新
            characterStates[dict.Key] = dict.Value.trainRecord.state;
        }
    }

    void OnDestroy()
    {
        // 取消訂閱訓練 UI 的開關事件
        DoorInteraction.OnDoorInteracted -= OpenOneUI;
        TrainingUIHandler.OnTrainingUIClosed -= OneOneUIClosed;
    }

    private void OpenOneUI(TrainingUIData data)
    {
        if (isInitialized == false) 
        {
            // 第一次開啟訓練 UI 時都能初始化隊伍資料
            InitializeTeamData();
            isInitialized = true;
        }

        TrainingType type = data.trainingType; // None=-1, Dance=0, Vocal=1, Visual=2

        TrainingUIHandler ui = type switch // 預計要打開的 UI
        {
            TrainingType.Dance => trainingUIHandlers[0],
            TrainingType.Vocal => trainingUIHandlers[1],
            TrainingType.Visual => trainingUIHandlers[2],
            _ => null
        };

        if (ui == null)
        {
            Debug.LogError("找不到對應的 TrainingUIHandler");
            return;
        }

        current = ui;
        ui.ShowTrainingUI(data);

        UIAndPlayerInput.DisableAllPlayerInputs(); // UI 已開啟 -> 禁用所有玩家移動
    }

    private void OneOneUIClosed()
    {
        current = null; // 目前沒有任何 UI 開著
        UIAndPlayerInput.EnableAllPlayerInputs(); // UI 已關閉 -> 啟用所有玩家移動
    }

    public IdolTrainingState GetIdolState(IdolWho idol) // 根據角色名取得其訓練狀態
    {
        return characterStates.ContainsKey(idol)
            ? characterStates[idol]
            : IdolTrainingState.None;
    }

    public void SetIdolState(IdolWho idol, IdolTrainingState state) // 設定角色的訓練狀態
    {
        characterStates[idol] = state;

        // 同步更新 IdolInstance 的 trainRecord（備份用）
        TraineeAssignment.UpdateTrainRecord(idol, state);
    }

    public List<IdolWho> GetMembers() // 取得目前隊伍成員清單
    {
        static bool IsInTeamScope(IdolTrainingState state)
        {
            return state == IdolTrainingState.InTeam || state == IdolTrainingState.Unable;
        }

        return characterStates
            .Where(x => IsInTeamScope(x.Value))
            .Select(x => x.Key)
            .ToList();
    }

    // public List<IdolWho> GetTrainees() // 取得目前所有訓練角色清單
    // {
    //     return characterStates
    //         .Where(x => x.Value != IdolTrainingState.InTeam && x.Value != IdolTrainingState.Unable)
    //         .Select(x => x.Key)
    //         .ToList();
    // }

    // public List<IdolWho> GetTrainees(IdolTrainingState room) // 取得目前在特定訓練室的角色清單
    // {
    //     return characterStates
    //         .Where(x => x.Value == room)
    //         .Select(x => x.Key)
    //         .ToList();
    // }
}

