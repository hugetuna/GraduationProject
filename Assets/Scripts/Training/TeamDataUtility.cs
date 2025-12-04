using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/* 提供隊伍相關的資料存取，不用特別掛在什麼地方 */
public class TeamDataUtility
{
    public static readonly int idolCount = 3; // 實際角色數量（不計隱藏情況）
    private static readonly Dictionary<string, IdolWho> NameToEnum
        = new()
    {
        { "Kuma", IdolWho.Kuma },
        { "Karo", IdolWho.Karo },
        { "Sirius", IdolWho.Sirius }
    };

    public static IdolWho GetIdolEnum(string name)
    {
        if (NameToEnum.TryGetValue(name, out IdolWho idol)) return idol;
        else{
            Debug.LogWarning($"找不到對應的角色 enum 值，名稱：{name}");
            return IdolWho.none;
        }
    }

    private static SortedDictionary<IdolWho, IdolInstance> idolDict;
    public static SortedDictionary<IdolWho, IdolInstance> IdolDict
    {
        get
        {
            if (idolDict == null || idolDict.Count == 0 || idolDict.Any(kv => kv.Value == null))
            {
                RefreshIdolInstances();
            }
            return idolDict;
        }
    }


    public static List<IdolInstance> IdolInstanceList
    {
        get
        {
            return IdolDict.Select(kv => kv.Value).ToList();
        }
    }

    public static List<GameObject> IdolObjectList
    {
        get
        {
            return IdolDict.Select(kv => kv.Value.gameObject).ToList();
        }
    }

    public static void RefreshIdolInstances()
    {
        var instances = Object.FindObjectsByType<IdolInstance>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var dict = instances.ToDictionary(i => i.idolIndex, i => i);
        idolDict = new SortedDictionary<IdolWho, IdolInstance>(dict);

        Debug.Log($"已刷新角色資料，場景中目前共有 {idolDict.Count} 個角色");
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
