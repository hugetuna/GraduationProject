using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUIHandler : MonoBehaviour
{
    public GameObject TraningUI;

    void Start()
    {
        TraningUI.SetActive(false);
        DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 訂閱並監聽事件
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 取消訂閱事件
    }

    void ShowTrainingUI(){
        Debug.Log("開啟訓練 UI");
        TraningUI.SetActive(true);

    }
}
