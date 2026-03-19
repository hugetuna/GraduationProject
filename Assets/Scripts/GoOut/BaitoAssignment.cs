using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在 UIManager 上 */
public class BaitoAssignment : MonoBehaviour
{
    public static List<IdolWho> assignedIdols = new(); // 欲指派商演的角色列表

    void Start()
    {
        SetBaitoUI.OnBaitoConfirmed += AssignToBaito;
    }

    void OnDestroy()
    {
        SetBaitoUI.OnBaitoConfirmed -= AssignToBaito;
    }
    
    public void AssignToBaito()
    {
        
        
    }
}
