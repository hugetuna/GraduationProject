using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerButton : MonoBehaviour
{
    private Button powerButton;
    public GameObject desktopUI;

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
        desktopUI.SetActive(false); // 關閉電腦桌面 UI
    }
}
