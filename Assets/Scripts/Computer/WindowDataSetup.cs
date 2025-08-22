using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 放在視窗 prefab 根部 */
public class WindowDataSetup : MonoBehaviour
{
    // 場景資料與視窗 prefab 內部的溝通介面
    private static ResourceManager resourceManager;

    public static void SetResourceManager(ResourceManager resourceManager)
    {
        WindowDataSetup.resourceManager = resourceManager;
    }

    public static ResourceManager GetResourceManager()
    {
        return resourceManager;
    }
}
