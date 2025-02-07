using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static int date = 1; // 靜態變數，保存遊戲中的日期

    // 用來更動日期的函式
    public void ChangeDay()
    {
        date++;
        Debug.Log($"今天是第 {date} 天");
    }

    // 可以在開始遊戲時初始化，或每次遊戲結束時進行保存
    void Start()
    {
        // 初始化日期，若需要可以改成從保存中讀取
        date = 1;
    }
}
