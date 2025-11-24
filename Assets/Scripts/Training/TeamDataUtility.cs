using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/* 提供隊伍相關的資料存取，不用特別掛在什麼地方 */
public class TeamDataUtility : MonoBehaviour
{
    private static Dictionary<string, IdolInstance> idolInstances = new();
    public static Dictionary<string, IdolInstance> IdolInstances
    {
        get
        {
            if (idolInstances.Count == 0 || idolInstances.Values.Any(input => input == null))
            {
                RefreshIdolInstances();
            }
            return idolInstances;
        }
    }

    public static void RefreshIdolInstances()
    {
        var instances = FindObjectsByType<IdolInstance>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        idolInstances = instances.ToDictionary(i => CleanNameOfCharacterObject(i.name), i => i);
        // Debug.Log($"刷新 IdolInstances，共 {idolInstances.Count} 個角色");
    }

    public static string CleanNameOfCharacterUI(string raw) // 移除角色 UI 圖片檔名中的多餘字串
    {
        return raw.Replace("UI_character_", "");
    }

    public static string CleanNameOfCharacterObject(string raw) // 移除角色物件名稱中的多餘字串
    {
        return raw.Replace("Character_", "").Replace("2.0", "").Replace("(Clone)", "").Trim();
    }
}