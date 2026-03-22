using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityAssignment : MonoBehaviour
{
    // 全員一起去商演，若有人體力不足會減少收益

    void Start()
    {
        SetActivityUI.OnActivityConfirmed += AssignToActivity;
    }

    void OnDestroy()
    {
        SetActivityUI.OnActivityConfirmed -= AssignToActivity;
    }
    
    public void AssignToActivity()
    {
        Debug.Log("指派外出商演");
        
    }
}
