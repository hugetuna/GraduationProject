using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 掛在 TrainingManager 上，負責指派訓練成員並備份資料 */
public class TraineeAssignment : MonoBehaviour
{
    private List<IdolWho> trainees = new();
    private List<GameObject> disappearCharacters = new(); // 需要隱藏的角色物件 
    [SerializeField] private TeamManager teamManager;

    void Start()
    {
        TrainingUIHandler.OnTrainingUIConfirmed += AssignTrainees; // 訂閱訓練 UI 確定指派事件
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIConfirmed -= AssignTrainees; // 取消訂閱訓練 UI 確定指派事件

    }

    public void AssignTrainees(TrainingUIData data) // 指派訓練成員的函式（確定有人才呼叫）
    {
        // 清除上一次的狀態
        foreach (var idol in TeamDataUtility.IdolObjectList){
            teamManager.RemoveBusyMember(idol.GetComponent<PlayerControlMainWorld>()); // 從忙碌成員列表移除
            idol.SetActive(true); // 將隱藏的角色都顯示出來 

            string name = TeamDataUtility.CleanNameOfCharacterObject(idol.name);
            UpdateTrainRecord(TeamDataUtility.GetIdolEnum(name), isActive: true); // 重設跨場景角色啟用狀態
        }
        disappearCharacters.Clear();

        // 取得目前 trainees 
        trainees = TrainingUIManager.Instance.GetTrainees();
        if(trainees.Count == 0)
        {
            return; // 若無人去訓練，就什麼也不做
        }
        Debug.Log($"指派訓練成員: {string.Join(", ", trainees)}");

        // 處理隊長 => 交由 TeamManager 內部處理
        // var leader = tm.teamMembers[tm.currentLeaderIndex];
        // bool leaderInTrainees = trainees.Any(t => leader.name.Contains(t));
        // if (leaderInTrainees && tm.teamMembers.Count > 1)
        // {
        //     // 若派隊長去訓練，且隊伍人數大於 1，則更換隊長
        //     tm.SwitchLeader(1);
        // }

        // 遍歷 trainees 以進行訓練指派 
        foreach (IdolWho trainee in trainees)
        {
            IdolInstance idol = TeamDataUtility.IdolDict[trainee];
            GameObject idolObject = idol.gameObject;
            PlayerControlMainWorld idolControl = idolObject.GetComponent<PlayerControlMainWorld>();
            
            if (idol == null)
            {
                Debug.Log($"找不到欲訓練的角色 {trainee}");
                continue;
            }

            // 同步更新 IdolInstance 的 trainRecord（備份用，還不會真的增減角色數值）
            int benefit = data.withTeacherBenefit; // 先假設都有老師加成
            switch (data.trainingType.ToLower())
            {
                case "dance":
                    benefit = (int)(benefit * idol.daTrainingBonus);
                    UpdateTrainRecord(trainee, vigourCost: data.neededVigour, dance: benefit, isActive: false);
                    break;
                case "vocal":
                    benefit = (int)(benefit * idol.voTrainingBonus);
                    UpdateTrainRecord(trainee, vigourCost: data.neededVigour, vocal: benefit, isActive: false);
                    break;
                case "visual":
                    benefit = (int)(benefit * idol.viTrainingBonus);
                    UpdateTrainRecord(trainee, vigourCost: data.neededVigour, visual: benefit, isActive: false);
                    break;
            }

            // 隱藏隊伍中去訓練的角色 
            Debug.Log($"隱藏訓練成員: {idolControl}");
            teamManager.AddBusyMember(idolControl);
            
            disappearCharacters.Add(idolObject);
            idolObject.SetActive(false);
        }

        // 更新成員們的 positionInTeam => 交由 TeamManager 內部處理
        // for (int i = 0; i < teamMembers.Count; i++)
        // {
        //     var idol = teamMembers[i].GetComponent<IdolInstance>();
        //     idol.positionInTeam = i;
        // }
    }

    // 在指派訓練成員的同時備份狀態變化
    public static void UpdateTrainRecord(IdolWho name, // 第一項引數必填
                                         IdolTrainingState state = IdolTrainingState.None,
                                         Vector2? position = null,
                                         int? vigourCost = null,
                                         int? dance = null,
                                         int? vocal = null,
                                         int? visual = null,
                                         bool? isActive = null)
    {
        var trainRecord = TeamDataUtility.IdolDict[name].trainRecord;

        if (state != IdolTrainingState.None) trainRecord.state = state; // TrainingUIManager
        if (position != null) trainRecord.position = position.Value; // DragToLesson
        if (vigourCost != null) trainRecord.vigourCost = vigourCost.Value; // TraineeAssignment
        if (dance != null) trainRecord.danceExp = dance.Value; // TraineeAssignment
        if (vocal != null) trainRecord.vocalExp = vocal.Value; // TraineeAssignment
        if (visual != null) trainRecord.visualExp = visual.Value; // TraineeAssignment
        if (isActive != null) trainRecord.isActive = isActive.Value; // TraineeAssignment
    }
}