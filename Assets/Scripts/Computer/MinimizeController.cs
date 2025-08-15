using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在最小化按鈕的 prefab 上，以控制視窗的最小化動畫 */
public class MinimizeController : MonoBehaviour
{
    private GameObject appWindow; // （由負責生成該物件的 SetAppUI 指派）
    private MinimizeAnimation minimizeAnimation; // 最小化動畫控制器（可由 appWindow 取得）
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ToggleMinimize);
    }

    public void ToggleMinimize()
    {
        if (minimizeAnimation != null)
        {
            if (appWindow.activeSelf) // 最小化關閉視窗
            {
                minimizeAnimation.Minimize();
            }
            else // 開啟最小化視窗
            {
                minimizeAnimation.Restore();
            }
        }
    }

    public void SetAppWindow(GameObject window)
    {
        appWindow = window;
        minimizeAnimation = appWindow.GetComponent<MinimizeAnimation>();
    }
}
