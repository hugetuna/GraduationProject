using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在 UIManager 上 */
public class BaitoAssignment : MonoBehaviour
{
    // [SerializeField] private List<DragToBaito> dragToBaito = new(); // 拖曳腳本，用來判斷角色所在區域
    [SerializeField] private TeamManager teamManager; // 用來標記忙碌角色（跨場景同步）

    void Start()
    {
        SetBaitoUI.OnBaitoConfirmed += AssignToBaito;
    }

    void OnDestroy()
    {
        SetBaitoUI.OnBaitoConfirmed -= AssignToBaito;
    }

    public void AssignToBaito(bool areAllToBaito)
    {
        foreach (var idol in TeamDataUtility.IdolInstanceList)
        {
            // 不應該出現在打工介面的角色就直接跳過（比如說她正好在訓練）
            if (!idol.CanShowInTheAction(AvailableAction.Baito)) continue;

            var control = idol.GetComponent<PlayerControlMainWorld>();

            // 因為角色的打工類型等資料已經在拖曳腳本裡更新了，所以這裡可以直接指派
            if (areAllToBaito)
            {
                // 在場景中顯示，但還是登記為打工中
                teamManager.RemoveBusyMember(control);
                idol.gameObject.SetActive(true);
                // 備份
                // idol.baitoRecord.SetBaitoRecord(idol.baitoRecord.selectedBaito);
                idol.isAvailable = true;
                idol.currentAction = AvailableAction.Baito;
            }
            else if (idol.baitoRecord.zoneType == BaitoDropZoneType.Baito)
            {
                // 在場景中隱藏
                teamManager.AddBusyMember(control);
                idol.gameObject.SetActive(false);
                // 備份
                // idol.baitoRecord.SetBaitoRecord(idol.baitoRecord.selectedBaito);
                idol.isAvailable = false;
                idol.currentAction = AvailableAction.Baito;
            }
            else if (idol.baitoRecord.zoneType == BaitoDropZoneType.Member)
            {
                // 在場景中顯示
                teamManager.RemoveBusyMember(control);
                idol.gameObject.SetActive(true);
                // 備份
                // idol.baitoRecord.SetBaitoRecord(idol.baitoRecord.selectedBaito);
                idol.isAvailable = true;
                idol.currentAction = AvailableAction.Free;
            }
        }
    }
}

