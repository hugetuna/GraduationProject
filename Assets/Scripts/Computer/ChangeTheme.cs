using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在 UIManager 之類的物件上 */
public class ChangeTheme : MonoBehaviour
{
    [Header("影響範圍")]
    public GameObject taskbar;
    public GameObject apps;
    private GameObject theMinimized;
    private GameObject powerButton;
    //-----------------------------------------------------------------//
    [Header("桌面主題設定")]
    public Sprite demonMiniBg; // 惡魔 UI 中最小化圖示的背景圖片
    public Sprite desktopMiniBg; // 桌面 UI 中最小化圖示的背景圖片
    private Color demonTbColor = new Color32(175, 129, 255, 255); // 惡魔 UI 工作列顏色
    private Color desktopTbColor = new Color32(255, 213, 252, 255); // 桌面 UI 工作列顏色
    private Color demonPbColor = new Color32(98, 0, 148, 255); // 惡魔 UI 電源鍵顏色
    private Color desktopPbColor = new Color32(255, 141, 211, 255); // 桌面 UI 電源鍵顏色

    void Start()
    {
        theMinimized = taskbar.transform.Find("TheMinimized").gameObject;
        powerButton = taskbar.transform.Find("PowerButton").gameObject;
    }

    public void SetDesktopTheme()
    {
        taskbar.GetComponent<Image>().color = desktopTbColor;

        apps.SetActive(true); // 顯示應用程式按鈕

        powerButton.GetComponent<Image>().color = desktopPbColor;

        Transform pt = theMinimized.transform;
        if (pt.childCount > 0)
        {
            foreach (Transform child in pt)
            {
                child.GetComponent<Image>().sprite = desktopMiniBg;
            }
        }

    }

    public void SetDemonTheme()
    {
        taskbar.GetComponent<Image>().color = demonTbColor;

        apps.SetActive(false); // 隱藏應用程式按鈕

        powerButton.GetComponent<Image>().color = demonPbColor;

        Transform parentTransform = theMinimized.transform;
        if (parentTransform.childCount > 0)
        {
            foreach (Transform child in parentTransform)
            {
                child.GetComponent<Image>().sprite = demonMiniBg;
            }
        }

    }
}
