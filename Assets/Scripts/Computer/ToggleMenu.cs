using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/* 掛在桌面工作列的開始按鈕上 */
public class MenuToggler : MonoBehaviour
{
    private Button startButton; // 自己身上的按鈕組件
    [SerializeField] private GameObject startMenu; // 開始選單物件
    [SerializeField] private Button panelToClose;

    void Awake()
    {
        startButton = GetComponent<Button>();
    }

    void Start()
    {
        startMenu.SetActive(false); // 初始隱藏開始選單

        startButton.onClick.AddListener(() =>
        {
            // 切換開始選單的顯示狀態
            startMenu.SetActive(!startMenu.activeSelf);
        });

        panelToClose.onClick.AddListener(() =>
        {
            // 點擊面板外圍關閉選單
            startMenu.SetActive(false);
        });
    }
}
