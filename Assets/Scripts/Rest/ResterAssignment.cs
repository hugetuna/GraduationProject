using UnityEngine;

public class ResterAssignment : MonoBehaviour
{
    [SerializeField] private TeamManager teamManager;

    void Start()
    {
        RestUIHandler.OnRestConfirmed += AssignResters;
    }

    void OnDestroy()
    {
        RestUIHandler.OnRestConfirmed -= AssignResters;
    }

    public void AssignResters(bool areAllToRest) // 當任意休息 UI 按下確定按鈕時呼叫
    {
        // 遍歷所有角色來檢查狀態
        foreach (var idol in TeamDataUtility.IdolInstanceList)
        {
            // 如果角色根本不應該出現在休息室介面，就直接跳過（比如說她正好在打工）
            if (!idol.CanShowInTheAction(AvailableAction.Rest)) continue;

            var control = idol.GetComponent<PlayerControlMainWorld>();
            var restRecord = idol.restRecord;

            // 因為角色的位置資料等等已經在拖曳腳本裡更新了，所以這裡可以直接指派
            if (areAllToRest)
            {
                // 在場景中顯示，但還是登記為打休息中
                teamManager.RemoveBusyMember(control);
                idol.gameObject.SetActive(true);
                idol.isAvailable = true;
                idol.currentAction = AvailableAction.Rest;
            }
            else if (restRecord.zoneType == RestDropZoneType.Rest)
            {
                // 在場景中隱藏
                teamManager.AddBusyMember(control);
                idol.gameObject.SetActive(false);
                idol.isAvailable = false;
                idol.currentAction = AvailableAction.Rest;
            }
            else if (restRecord.zoneType == RestDropZoneType.Member)
            {
                // 在場景中顯示
                teamManager.RemoveBusyMember(control);
                idol.gameObject.SetActive(true);
                idol.isAvailable = true;
                idol.currentAction = AvailableAction.Free;
            }
        }
    }
}
