using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // 如果角色在打工區，則指派外出打工
            if (drag.CurrentDropZone.zoneType == BaitoDropZoneType.Baito)
            {
                var idol = TeamDataUtility.IdolDict[drag.MyIdolIndex];
                var baitoRecord = idol.baitoRecord;
                var control = idol.GetComponent<PlayerControlMainWorld>();
                
                var assignEffect = drag.GetComponent<BaitoAssignEffect>();
                
                // 隱藏場景中的角色
                teamManager.AddBusyMember(control); // 標記為忙碌並隱藏角色物件
                idol.gameObject.SetActive(false);

                // 已派出的角色＆體力條 => 降低圖片透明度且不得拖曳 + 文字提示
                assignEffect.UpdateCharacterStatus(selectedBaito, !baitoRecord.isWorking);
                
                // 跨場景存檔
                baitoRecord.selectedBaito = selectedBaito;
                baitoRecord.isWorking = true;
                idol.isAvailable = false;

                // 播放音效
                if (goBaitoSound != null) AudioManager.Instance.PlaySFX(goBaitoSound);
            }
            // 若不在打工區，則不做任何處理（不過玩家可以分批指派角色去打工）
        }
    }
}
