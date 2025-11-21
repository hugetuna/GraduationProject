using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 掛在 TrainingManager 上 */
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
        List<PlayerControlMainWorld> teamMembers = tm.teamMembers;

        // 先將之前隱藏的角色顯示出來 
        foreach (var character in disappearCharacters) character.SetActive(true);
        disappearCharacters.Clear(); // 清空上一次的列表 

        // 取得目前 trainees 
        trainees = TrainingUIManager.Instance.GetTrainees();
        Debug.Log("指派訓練成員: " + string.Join(", ", trainees));
        if (trainees.Count == 0) return;
        if (trainees.Count == teamMembers.Count)
        {
            Debug.LogWarning("無法全部成員同時訓練，至少保留一名成員在隊伍中！");
            return;
        }

        // 處理隊長 
        var leader = teamMembers[tm.currentLeaderIndex];
        bool leaderInTrainees = trainees.Any(t => leader.name.Contains(t));
        if (leaderInTrainees && tm.teamMembers.Count > 1)
        {
            // 若派隊長去訓練，且隊伍人數大於 1，則更換隊長
            tm.SwitchLeader(1);
        }

        // 遍歷 trainees 以進行訓練指派 
        foreach (string trainee in trainees)
        {
            IdolInstance idolInstance = System.Array.Find(
                TeamDataUtility.IdolInstances,
                obj => obj.name.Contains(trainee)
            );
            if (idolInstance == null)
            {
                Debug.Log($"找不到欲訓練的角色 {trainee}");
                continue;
            }

            // 這裡還不會真的加角色數值 
            // float bonus = string.IsNullOrEmpty(data.teacherName) ? data.basicBenefit : data.withTeacherBenefit;
            // switch (data.trainingType)
            // {
            //     case "Dance": idolInstance.dance += (int)(bonus * idolInstance.daTrainingBonus); break;
            //     case "Vocal": idolInstance.vocal += (int)(bonus * idolInstance.voTrainingBonus); break;
            //     case "Visual": idolInstance.visual += (int)(bonus * idolInstance.viTrainingBonus); break;
            // }

            // 隱藏隊伍中去訓練的角色 
            var character = idolInstance.gameObject;
            disappearCharacters.Add(character);
            character.SetActive(false);
        }

        // 更新成員們的 positionInTeam 
        for (int i = 0; i < teamMembers.Count; i++)
        {
            var idol = teamMembers[i].GetComponent<IdolInstance>();
            idol.positionInTeam = i;
        }
    }
}