using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum IdolTrainingState
{
    InTeam,
    InDance,
    InVocal,
    InVisual,
    None
}

/* 掛在 TrainingManager 上，統一管理三種不同的 UI（Singleton） */
public class TrainingUIManager : MonoBehaviour
{
    public static TrainingUIManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [SerializeField] private List<TrainingUIHandler> trainingUIHandlers = new();
    private TrainingUIHandler current = null; // 目前開著的 UI
    //-----------------------------------------------------------------//
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private List<Sprite> characterSprites = new(); // 角色 UI 圖片
    private Dictionary<string, IdolTrainingState> characterStates = new(); // 角色名稱＆訓練狀態對應表
    private bool isInitialized = false;
    // 目前還沒有選角功能，不必特別照隊伍成員決定用哪些圖片


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

        // 設定角色的初始訓練狀態（也考慮了跨場景的情形）
        foreach (var member in teamManager.teamMembers)
        {
            if (member == null) continue;

            // 將物件名稱轉成 ID
            string id = TeamDataUtility.CleanNameOfCharacterObject(member.name);

            IdolInstance instance = member.GetComponent<IdolInstance>();
            if (instance == null) continue;

            // 直接覆蓋即可，Dictionary 本身會自動新增或更新
            characterStates[id] = instance.state;
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
            InitializeTeamData();
            isInitialized = true;
        }

        string type = data.trainingType.ToLower();

        TrainingUIHandler ui = type switch // 預計要打開的 UI
        {
            "dance" => trainingUIHandlers[0],
            "vocal" => trainingUIHandlers[1],
            "visual" => trainingUIHandlers[2],
            _ => null
        };

        if (ui == null)
        {
            Debug.LogError("找不到對應的 TrainingUIHandler");
            return;
        }

        current = ui;
        ui.ShowTrainingUI(data, teamManager, characterSprites);

        UIAndPlayerInput.DisableAllPlayerInputs(); // UI 已開啟 -> 禁用所有玩家移動
    }

    private void OneOneUIClosed(TeamManager team, TrainingUIData data)
    {
        // 雖然這個事件有兩個引數，但這邊剛好不會用到呢

        current = null; // 目前沒有任何 UI 開著
        UIAndPlayerInput.EnableAllPlayerInputs(); // UI 已關閉 -> 啟用所有玩家移動
    }

    public IdolTrainingState GetIdolState(string name) // 根據角色名取得其訓練狀態
    {
        return characterStates.ContainsKey(name)
            ? characterStates[name]
            : IdolTrainingState.InTeam;
    }

    public List<string> GetMembers() // 取得目前隊伍成員清單
    {
        return characterStates
            .Where(x => x.Value == IdolTrainingState.InTeam)
            .Select(x => x.Key)
            .ToList();
    }

    public List<string> GetTrainees() // 取得目前所有訓練角色清單
    {
        return characterStates
            .Where(x => x.Value != IdolTrainingState.InTeam)
            .Select(x => x.Key)
            .ToList();
    }

    public List<string> GetTrainees(IdolTrainingState room) // 取得目前在特定訓練室的角色清單
    {
        return characterStates
            .Where(x => x.Value == room)
            .Select(x => x.Key)
            .ToList();
    }

    public void SetIdolState(string name, IdolTrainingState state) // 設定角色的訓練狀態
    {
        if (!characterStates.ContainsKey(name)) characterStates.Add(name, state);
        else characterStates[name] = state;

        // 同步更新 IdolInstance 的 trainRecord（備份用）
        TraineeAssignment.UpdateTrainRecord(name, state);
    }
}

