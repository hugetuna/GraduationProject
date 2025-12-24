using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeUserType : MonoBehaviour
{
    [Header("用戶類型按鈕＆頁面切換")]

    [Tooltip("用戶分類按鈕（需與用戶分類頁面互相對應）")]
    public List<Button> userTypeButtons = new();

    [Tooltip("用戶分類頁面（需與用戶類型按鈕互相對應）")]
    public List<GameObject> userTypePages = new();
    // private int currentIndex = 0; // 當前選中的按鈕索引

    void Start()
    {
        // 設定按鈕的點擊事件
        foreach (Button btn in userTypeButtons)
        {
            Button tempBtn = btn; // 捕捉當下按鈕以避免閉包問題
            tempBtn.onClick.AddListener(() => OnButtonClick(tempBtn));
        }

        // 預設顯示第一個分類頁面，其他先隱藏
        for(int i = 0; i < userTypePages.Count; i++)
        {
            if (i == 0) userTypePages[i].SetActive(true);
            else userTypePages[i].SetActive(false);
        }
    }

    public void OnButtonClick(Button clickedButton)
    {
        for (int i = 0; i < userTypeButtons.Count; i++)
        {
            // 一般按鈕：隱藏頁面
            userTypePages[i].SetActive(false);
        }

        // 被按下的按鈕（唯一）：顯示對應的用戶頁面（目前分類有全部、好友、老師、資方）
        if (clickedButton == userTypeButtons[0])
        {
            userTypePages[0].SetActive(true);
            // currentIndex = 0; // 更新當前索引
        }
        else if (clickedButton == userTypeButtons[1])
        {
            userTypePages[1].SetActive(true);
            // currentIndex = 1; // 更新當前索引
        }
        else if (clickedButton == userTypeButtons[2])
        {
            userTypePages[2].SetActive(true);
            // currentIndex = 2; // 更新當前索引
        }
        else if (clickedButton == userTypeButtons[3])
        {
            userTypePages[3].SetActive(true);
            // currentIndex = 3; // 更新當前索引
        }
    }
}
