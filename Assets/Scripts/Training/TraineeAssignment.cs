using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在 TrainingManager 上 */
public class TraineeAssignment : MonoBehaviour
{
    private List<string> teamTrainees = new();
    private List<GameObject> disappearCharacters = new(); // 需要隱藏的角色物件 = 當前的 Trainee 們

    void Start()
    {
        TrainingUIHandler.OnTrainingUIClosed += AssignTrainees; // 訂閱訓練 UI 關閉事件
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIClosed -= AssignTrainees; // 取消訂閱與電腦互動事件
    }

    public void AssignTrainees(TeamManager teamManager, TrainingUIData data) // 指派訓練成員的函式
    {
        // 清空先前的隱藏角色清單
        foreach (var character in disappearCharacters) character.SetActive(true);
        disappearCharacters.Clear();

        // 取得欲訓練的成員名單
        teamTrainees = data.teamData.GetTrainees();
        if (teamTrainees.Count == 0) return;
        else if (teamTrainees.Count == teamManager.teamMembers.Count)
        {
            Debug.LogWarning("無法全部成員同時訓練，至少保留一名成員在隊伍中！");
            return;
        }
        
        // 如果隊長在即將派出去的名單裡，先切換隊長
        bool leaderInTrainees = false;
        var leader = teamManager.teamMembers[teamManager.currentLeaderIndex];

        for (int i = 0; i < teamTrainees.Count; i++)
        {
            if (leader.name.Contains(teamTrainees[i]))
            {
                leaderInTrainees = true;
                break; // 找到就跳出
            }
        }

        // 如果隊長在訓練名單中，且隊伍中還有其他成員，則切換隊長
        if (leaderInTrainees && teamManager.teamMembers.Count > 1)
        {
            teamManager.SwitchLeader(1);
        }

        // 遍歷 trainee 名單
        foreach (string trainee in teamTrainees)
        {
            IdolInstance idolInstance = System.Array.Find(GlobalTeamData.IdolInstances, obj => obj.name.Contains(trainee));
            
            if (idolInstance == null)
            {
                Debug.Log($"找不到欲訓練的角色 {trainee}");
                continue;
            }

            // 扣體力（不確定實際上的扣除時機是什麼時候）
            // idolInstance.vigour -= data.neededVigour;

            // 增加對應的能力值
            if (data.trainingType == "Dance")
            {
                float bonus = data.teacherName == "" ? data.basicBenefit : data.withTeacherBenefit;
                idolInstance.dance += (int)(bonus * idolInstance.daTrainingBonus);
            }

            // 隱藏該角色物件
            var character = idolInstance.gameObject;
            disappearCharacters.Add(character);
            character.SetActive(false);

            // idolInstance.positionInTeam = -1;
        }

        // 更新剩下成員的 positionInTeam
        for (int i = 0; i < teamManager.teamMembers.Count; i++)
        {
            var idol = teamManager.teamMembers[i].GetComponent<IdolInstance>();
            idol.positionInTeam = i;
        }
    }
}
