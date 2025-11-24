using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 掛在 TrainingManager 上，負責指派訓練成員並備份資料 */
public class TraineeAssignment : MonoBehaviour
{
    private List<string> trainees = new();
    private List<GameObject> disappearCharacters = new(); // 需要隱藏的角色物件 

    void Start()
    {
        TrainingUIHandler.OnTrainingUIClosed += AssignTrainees; // 訂閱訓練 UI 關閉事件
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIClosed -= AssignTrainees; // 取消訂閱與電腦互動事件 

    }

    public void AssignTrainees(TeamManager tm, TrainingUIData data) // 指派訓練成員的函式 
    {
        foreach (var idol in TeamDataUtility.IdolInstances.Values.Select(i => i.gameObject)){
            idol.SetActive(true); // 將隱藏的角色都顯示出來 
            tm.RemoveBusyMember(idol.GetComponent<PlayerControlMainWorld>()); // 從忙碌成員列表移除
            UpdateTrainRecord(idol.name, isActive: true); // 重設跨場景角色啟用狀態
        }
        disappearCharacters.Clear(); // 清空上一次的列表 

        // 取得目前 trainees 
        trainees = TrainingUIManager.Instance.GetTrainees();
        Debug.Log("指派訓練成員: " + string.Join(", ", trainees));
        if (trainees.Count == 0) return;
        if (trainees.Count == tm.teamMembers.Count)
        {
            Debug.LogWarning("無法全部成員同時訓練，至少保留一名成員在隊伍中！");
            return;
        }

        // 處理隊長 
        var leader = tm.teamMembers[tm.currentLeaderIndex];
        bool leaderInTrainees = trainees.Any(t => leader.name.Contains(t));
        if (leaderInTrainees && tm.teamMembers.Count > 1)
        {
            // 若派隊長去訓練，且隊伍人數大於 1，則更換隊長
            tm.SwitchLeader(1);
        }

        // 遍歷 trainees 以進行訓練指派 
        foreach (string trainee in trainees)
        {
            IdolInstance idolInstance = TeamDataUtility.IdolInstances[trainee];
            if (idolInstance == null)
            {
                Debug.Log($"找不到欲訓練的角色 {trainee}");
                continue;
            }

            // 同步更新 IdolInstance 的 trainRecord（備份用，還不會真的增減角色數值）
            float bonus = string.IsNullOrEmpty(data.teacherName) ? data.basicBenefit : data.withTeacherBenefit;
            int benefit;
            switch (data.trainingType)
            {
                case "Dance":
                    benefit = (int)(bonus * idolInstance.daTrainingBonus);
                    UpdateTrainRecord(trainee, dance: benefit);
                    break;
                case "Vocal":
                    benefit = (int)(bonus * idolInstance.voTrainingBonus);
                    UpdateTrainRecord(trainee, vocal: benefit);
                    break;
                case "Visual":
                    benefit = (int)(bonus * idolInstance.viTrainingBonus);
                    UpdateTrainRecord(trainee, visual: benefit);
                    break;
            }
            UpdateTrainRecord(trainee, vigourCost: data.neededVigour, isActive: false);

            // 隱藏隊伍中去訓練的角色 
            var character = idolInstance.gameObject;
            disappearCharacters.Add(character);
            tm.RemoveBusyMember(character.GetComponent<PlayerControlMainWorld>()); // 從忙碌成員列表移除
            character.SetActive(false);
        }

        // 更新成員們的 positionInTeam 
        // for (int i = 0; i < teamMembers.Count; i++)
        // {
        //     var idol = teamMembers[i].GetComponent<IdolInstance>();
        //     idol.positionInTeam = i;
        // }
    }

    // 在指派訓練成員的同時備份狀態變化
    public static void UpdateTrainRecord(string name, IdolTrainingState state = IdolTrainingState.None,
                                    Vector2? position = null,
                                    int? vigourCost = null,
                                    int? dance = null,
                                    int? vocal = null,
                                    int? visual = null,
                                    bool? isActive = null)
    {
        var idol = TeamDataUtility.IdolInstances[name];

        if (state != IdolTrainingState.None) idol.state = state;
        if (position != null) idol.positionInTrainingUI = position.Value;
        if (vigourCost != null) idol.vigourCost = vigourCost.Value;
        if (dance != null) idol.danceExp = dance.Value;
        if (vocal != null) idol.vocalExp = vocal.Value;
        if (visual != null) idol.visualExp = visual.Value;
        if (isActive != null) idol.isActive = isActive.Value;
    }
}