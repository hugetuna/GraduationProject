using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum AppTypes { Store, Chat, Appoint, Sell, GuideBook }

[CreateAssetMenu(fileName = "AppData", menuName = "Computer/AppData")]
public class AppData : ScriptableObject
{
    public string appName;  // 應用程式名稱（目前有商店、聊天室、預約、販賣與圖鑑等）
    public Sprite appIcon; // 應用程式圖示
    public GameObject appWindowPrefab; // 對應的視窗 Prefab
    //private GameObject appPrefab; // 對應的應用程式 Prefab
    // public AppTypes appType; // 應用程式類型
}