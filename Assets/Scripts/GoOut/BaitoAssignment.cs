using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在 UIManager 上 */
public class BaitoAssignment : MonoBehaviour
{
    [SerializeField] private List<DragToBaito> dragToBaito = new(); // 拖曳腳本，用來判斷角色所在區域
    [SerializeField] private TeamManager teamManager; // 用來標記忙碌角色（跨場景同步）
    [SerializeField] private AudioClip goBaitoSound; // 打工出發音效

    void Start()
    {
        SetBaitoUI.OnBaitoConfirmed += AssignToBaito;
    }

    void OnDestroy()
    {
        SetBaitoUI.OnBaitoConfirmed -= AssignToBaito;
    }

    public void AssignToBaito(Baito selectedBaito, bool areAllToBaito)
    {
        foreach (var drag in dragToBaito)
        {
            var idol = TeamDataUtility.IdolDict[drag.MyIdolIndex];

            bool isInBaitoZone = drag.CurrentDropZone.zoneType == BaitoDropZoneType.Baito;
            bool isAssigned = areAllToBaito || isInBaitoZone; // 是否計算打工數值
            bool isVisibleInWorld = areAllToBaito || !isInBaitoZone; // 是否在場景中顯示

            bool isActive = isVisibleInWorld && idol.isAvailable; // 需考慮角色是否在隊伍裡
            UpdateBaitoStatus(idol, isAssigned ? selectedBaito : null, isActive);
        }


        // 播放音效
        if (goBaitoSound != null) AudioManager.Instance.PlaySFX(goBaitoSound);
    }

    public void UpdateBaitoStatus(IdolInstance idol, Baito baitoData, bool isActive)
    {
        var control = idol.GetComponent<PlayerControlMainWorld>();

        if (isActive)
        {
            teamManager.RemoveBusyMember(control);
            idol.gameObject.SetActive(true);
        }
        else
        {
            teamManager.AddBusyMember(control);
            idol.gameObject.SetActive(false);
        }

        idol.baitoRecord.SetBaitoRecord(baitoData);
        idol.isAvailable = isActive;
    }
}

