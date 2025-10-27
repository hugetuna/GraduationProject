using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class PowerButton : MonoBehaviour
{
    private Button powerButton;
    [SerializeField] private GameObject desktopUI;
    [SerializeField] private GameObject startMenu;

    void Start()
    {
        powerButton = GetComponent<Button>(); // 獲取自己底下的按鈕組件
        powerButton.onClick.AddListener(TurnOffComputer); // 設置按鈕點擊事件
    }

    void Update()
    {

    }
    
    private void TurnOffComputer()
    {
        startMenu.SetActive(false); // 關閉開始選單
        desktopUI.SetActive(false); // 關閉電腦桌面 UI

        // 切換成透視投影
        Camera.main.orthographic = false;
    }
}
