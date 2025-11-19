using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 掛在 TrainingManager 上，統一管理三種不同的 UI（Singleton） */
public class TrainingUIManager : MonoBehaviour
{
    public static TrainingUIManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [SerializeField] private List<TrainingUIHandler> trainingUIHandlers = new();
    private TrainingUIHandler current = null; // 目前開著的 UI
    //-----------------------------------------------------------------//
    [SerializeField] private TeamManager teamManager;
    private List<string> members = new(); // 儲存當前隊伍成員
    private List<string> trainees = new(); // 儲存當前訓練成員
    [SerializeField] private List<Sprite> characterSprites = new(); // 角色 UI 圖片
    // 目前還沒有選角功能，不必特別照隊伍成員決定用哪些圖片
    

    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    void Start()
    {
        InitializeTeamData();

        // 訂閱訓練 UI 的開關事件
        DoorInteraction.OnDoorInteracted += OpenOneUI;
        TrainingUIHandler.OnTrainingUIClosed += OneOneUIClosed;
    }

    public void InitializeTeamData()
    {
        // 初始化當前隊伍成員
        members = teamManager.teamMembers
            .Select(m => TeamDataUtility.CleanNameOfCharacterObject(m.name))
            .ToList();

        trainees.Clear(); // 一開始沒有訓練成員
    }

    void OnDestroy()
    {
        // 取消訂閱訓練 UI 的開關事件
        DoorInteraction.OnDoorInteracted -= OpenOneUI;
        TrainingUIHandler.OnTrainingUIClosed -= OneOneUIClosed;
    }

    private void OpenOneUI(TrainingUIData data)
    {
        string type = data.trainingType.ToLower();
        TrainingUIHandler ui; // 預計要打開的訓練 UI

        switch (type)
        {
            case "dance":
                ui = trainingUIHandlers[0];
                break;
            case "vocal":
                ui = trainingUIHandlers[1];
                break;
            case "visual":
                ui = trainingUIHandlers[2];
                break;
            default:
                ui = null;
                break;
        }

        current = ui;
        ui.ShowTrainingUI(data, teamManager, characterSprites);
    }

    private void OneOneUIClosed(TeamManager team, TrainingUIData data)
    {
        current = null; // 目前沒有任何 UI 開著
        // 雖然這個事件有兩個引數，但這邊剛好不會用到呢
    }

    // 取得當前隊伍成員
    public List<string> GetMembers() => members;

    // 取得當前訓練成員
    public List<string> GetTrainees() => trainees;

    // 新增訓練成員（將其移出隊伍）
    public void AddTrainee(string name)
    {
        if (!trainees.Contains(name))
        {
            trainees.Add(name);
            members.Remove(name);
        }
    }

    // 移除訓練成員（將其移回隊伍）
    public void RemoveTrainee(string name)
    {
        if (trainees.Contains(name))
        {
            trainees.Remove(name);
            members.Add(name);
        }
    }
}

