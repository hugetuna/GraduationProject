using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在 UIManager 上 */
public class MainUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    void Start()
    {
        ComputerInteraction.OnComputerInteracted += HideMainUI; // 訂閱並監聽與電腦互動事件
    }

    void OnDestroy()
    {
        ComputerInteraction.OnComputerInteracted -= HideMainUI; // 取消訂閱與電腦互動事件
    }

    private void HideMainUI()
    {
        mainUI.SetActive(false);
    }
}
