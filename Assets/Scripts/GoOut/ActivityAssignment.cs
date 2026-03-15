using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在 UIManager 上 */
public class ActivityAssignment : MonoBehaviour
{
    public static List<IdolWho> assignedIdols = new(); // 欲指派商演的角色列表

    void Start()
    {
        ActivityUIHandler.OnActivityConfirmed += AssignToActivity;
    }

    void OnDestroy()
    {
        ActivityUIHandler.OnActivityConfirmed -= AssignToActivity;
    }
    
    public void AssignToActivity()
    {
        
        
    }
}
