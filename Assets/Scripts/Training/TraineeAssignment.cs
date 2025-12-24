using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 掛在 TrainingManager 上，負責指派訓練成員並備份資料 */
public class TraineeAssignment : MonoBehaviour
{
    private List<IdolWho> currentTraineeList = new(); // 當前正在訓練的成員名單
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private AudioClip assignSound;

    void Start()
    {
        TrainingUIHandler.OnTrainingUIConfirmed += AssignTrainees;
        HintToggler.OnGoToComputer += AssignTrainees;
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIConfirmed -= AssignTrainees;
        HintToggler.OnGoToComputer -= AssignTrainees;
    }

    public void AssignTrainees(TrainingUIData data)
    {
        if(assignSound != null) AudioManager.Instance.PlaySFX(assignSound);

        // 1. 取得 UI 目前選中的訓練名單
        List<IdolWho> newTraineeList = TrainingUIManager.Instance.GetTrainees();

        // 2. 處理「被移出訓練」的人：在舊清單但不在新清單
        var removedTrainees = currentTraineeList.Except(newTraineeList).ToList();
        foreach (var trainee in removedTrainees)
        {
            ResetTraineeToWorld(trainee);
        }

        // 3. 檢查是否有任何人需要受訓
        if (newTraineeList.Count == 0)
        {
            currentTraineeList.Clear();
            return;
        }

        // 判斷是否全體受訓（影響隱藏邏輯）
        bool isAllTrainees = newTraineeList.Count == TeamDataUtility.idolCount;

        // 4. 遍歷新名單：處理「新加入」的體力扣除與「所有受訓者」的紀錄更新
        foreach (IdolWho traineeWho in newTraineeList)
        {
            IdolInstance idol = TeamDataUtility.IdolDict[traineeWho];

            // A. 如果是「新加入」的人，扣除體力
            if (!currentTraineeList.Contains(traineeWho))
            {
                idol.costVigour(data.neededVigour);
                // Debug.Log($"{traineeWho} 加入訓練，預扣體力: {data.neededVigour}");
            }

            // B. 計算加成數值並更新備份紀錄 (trainRecord)
            int benefit = CalculateBenefit(idol, data);
            UpdateTrainRecord(traineeWho, vigourCost: data.neededVigour,
                              danceExp: data.trainingType.ToLower() == "dance" ? benefit : 0,
                              vocalExp: data.trainingType.ToLower() == "vocal" ? benefit : 0,
                              visualExp: data.trainingType.ToLower() == "visual" ? benefit : 0,
                              isActive: false);

            // C. 處理場景角色隱藏邏輯
            HandleCharacterVisibility(traineeWho, isAllTrainees);
        }

        // 5. 更新本地暫存名單
        currentTraineeList = new List<IdolWho>(newTraineeList);
    }

    // 將角色歸還回世界場景，並返還體力
    private void ResetTraineeToWorld(IdolWho name)
    {
        var idol = TeamDataUtility.IdolDict[name];

        // 若上次有被指派訓練，但被放回隊伍，則返還體力
        if (idol.trainRecord.vigourCost > 0)
        {
            idol.recoverVigour(idol.trainRecord.vigourCost);
            // Debug.Log($"{name} 移出訓練，返還體力: {idol.trainRecord.vigourCost}");
        }

        // 重置備份紀錄為初始狀態
        UpdateTrainRecord(name, vigourCost: 0, danceExp: 0, vocalExp: 0, visualExp: 0, isActive: true);

        // 恢復場景物件顯示
        GameObject idolObject = idol.gameObject;
        if (idolObject != null)
        {
            teamManager.RemoveBusyMember(idolObject.GetComponent<PlayerControlMainWorld>());
            idolObject.SetActive(true);
        }
    }

    // 處理角色物件的隱藏與 Busy 狀態
    private void HandleCharacterVisibility(IdolWho name, bool isAllTrainees)
    {
        if (isAllTrainees) return; // 若全體訓練，則不隱藏任何角色（並跳出是否切換至電腦場景的提示）

        GameObject idolObject = TeamDataUtility.IdolDict[name].gameObject;
        if (idolObject != null)
        {
            teamManager.AddBusyMember(idolObject.GetComponent<PlayerControlMainWorld>());
            idolObject.SetActive(false);
        }
    }

    // 計算訓練效益加成
    private int CalculateBenefit(IdolInstance idol, TrainingUIData data)
    {
        float bonus = 1.0f;
        switch (data.trainingType.ToLower())
        {
            case "dance": bonus = idol.daTrainingBonus; break;
            case "vocal": bonus = idol.voTrainingBonus; break;
            case "visual": bonus = idol.viTrainingBonus; break;
        }
        return (int)(data.withTeacherBenefit * bonus);
    }

    public static void UpdateTrainRecord(IdolWho name, 
                                         IdolTrainingState state = IdolTrainingState.None, 
                                         Vector2? position = null, int? vigourCost = null, 
                                         int? danceExp = null, 
                                         int? vocalExp = null, 
                                         int? visualExp = null, 
                                         bool? isActive = null)
    {
        var idol = TeamDataUtility.IdolDict[name];
        var trainRecord = idol.trainRecord;

        if (state != IdolTrainingState.None) trainRecord.state = state;
        if (position != null) trainRecord.position = position.Value;
        if (vigourCost != null) trainRecord.vigourCost = vigourCost.Value;
        if (danceExp != null) trainRecord.danceExp = danceExp.Value;
        if (vocalExp != null) trainRecord.vocalExp = vocalExp.Value;
        if (visualExp != null) trainRecord.visualExp = visualExp.Value;
        if (isActive != null) trainRecord.isActive = isActive.Value;
    }
}