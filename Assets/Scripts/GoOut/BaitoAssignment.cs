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

    public void AssignToBaito(Baito selectedBaito)
    {
        foreach (var drag in dragToBaito)
        {
            var idol = TeamDataUtility.IdolDict[drag.MyIdolIndex];
            var baitoRecord = idol.baitoRecord;
            var control = idol.GetComponent<PlayerControlMainWorld>();

            // 如果角色在打工區，則指派外出打工
            if (drag.CurrentDropZone.zoneType == BaitoDropZoneType.Baito)
            {
                // 隱藏場景中的角色
                teamManager.AddBusyMember(control); // 標記為忙碌並隱藏角色物件
                idol.gameObject.SetActive(false);

                // 跨場景存檔
                baitoRecord.selectedBaito = selectedBaito;
                idol.isAvailable = false;
            }
            // 若不在打工區，將角色送回隊伍
            else
            {
                // 顯示場景中的角色
                teamManager.RemoveBusyMember(control); // 取消忙碌標記並顯示角色物件
                idol.gameObject.SetActive(true);

                // 跨場景存檔
                baitoRecord.selectedBaito = null;
                idol.isAvailable = true;
            }
        }

        // 播放音效
        if (goBaitoSound != null) AudioManager.Instance.PlaySFX(goBaitoSound);
    }
}
