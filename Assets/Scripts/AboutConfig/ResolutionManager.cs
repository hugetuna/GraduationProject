using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ResolutionManager : MonoBehaviour
{
    [Header("基本資料")]
    public List<Resolution> resolutionDatas;
    public int targetWidth = 1920;
    public int targetHeight = 1080;
    public FullScreenMode targetFullScreenMode = FullScreenMode.Windowed;
    [Header("ui元件")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown screenDropdown;
    void Start()
    {
        resolutionDatas = new List<Resolution>();
        CatchAvailableResolution();
        SetResolutionDropdownList();
        SetScreenModeDropdownList();
    }
    public void CatchAvailableResolution()
    {
        //1.抓取可用解析度
        Resolution[] resolutions = Screen.resolutions;
        resolutionDatas.Clear();
        // 2. 剔除重複的 (只看寬高，不管更新率)
        HashSet<string> seenResolutions = new HashSet<string>();//不會重複的資料結構
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resKey = resolutions[i].width + "x" + resolutions[i].height;
            if (!seenResolutions.Contains(resKey))
            {
                seenResolutions.Add(resKey);
                resolutionDatas.Add(resolutions[i]);
            }
        }
    }
    //設定解析度列表至下拉式選單
    public void SetResolutionDropdownList()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        // 3. 將解析度加入下拉式選單，順便設置當前值
        for (int i = 0; i < resolutionDatas.Count; i++)
        {
            string option = resolutionDatas[i].width + " x " + resolutionDatas[i].height;
            options.Add(option);
            if (resolutionDatas[i].width == targetWidth && resolutionDatas[i].height == targetHeight)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetGameResolutionByDropDown(int DropDownValue)
    {
        Resolution settingResolution = resolutionDatas[DropDownValue];
        targetWidth = settingResolution.width;
        targetHeight = settingResolution.height;
        Screen.SetResolution(targetWidth, targetHeight, targetFullScreenMode);
        print("設定解析度為 " + targetWidth + "x" + targetHeight + "，模式：" + targetFullScreenMode);
    }
    public void SetScreenModeDropdownList()
    {
        switch ((int)targetFullScreenMode)
        {
            case 0:
            case 2:
                screenDropdown.value = 0;
                break;
            case 1:
                screenDropdown.value = 1;
                break;
            case 3:
                screenDropdown.value = 2;
                break;
        }
        resolutionDropdown.RefreshShownValue();
    }
    public void SetFullScreenMode(int screenMode)
    {
        // 0:獨佔全螢幕, 1:全螢幕視窗, 2:視窗化
        switch (screenMode)
        {
            case 0:
                targetFullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                targetFullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                targetFullScreenMode = FullScreenMode.Windowed;
                break;
        }
        // 特殊處理 Windows 與 macOS 的全螢幕模式差異
        if (Application.platform == RuntimePlatform.WindowsPlayer&& targetFullScreenMode == FullScreenMode.MaximizedWindow)
        {
            targetFullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Debug.Log("正在 Windows 系統上執行！改換模式");
        }
        else if(Application.platform == RuntimePlatform.OSXPlayer && targetFullScreenMode == FullScreenMode.ExclusiveFullScreen)
        {
            targetFullScreenMode = FullScreenMode.MaximizedWindow;
            Debug.Log("正在 macOS 系統上執行！改換模式");
        }
        Screen.SetResolution(targetWidth, targetHeight, targetFullScreenMode);
        print("設定解析度為 " + targetWidth + "x" + targetHeight + "，模式：" + targetFullScreenMode);
    }
}
