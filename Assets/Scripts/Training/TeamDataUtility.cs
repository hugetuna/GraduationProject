using UnityEngine;

/* 提供隊伍相關的資料存取，不用特別掛在什麼地方 */
public class TeamDataUtility : MonoBehaviour
{
    private static IdolInstance[] idolInstances = null;

    public static IdolInstance[] IdolInstances
    {
        get
        {
            // 如果陣列不存在或裡面物件被刪掉，就刷新
            if (idolInstances == null)
            {
                RefreshIdolInstances();
            }
            return idolInstances;
        }
    }

    public static void RefreshIdolInstances()
    {
        idolInstances = FindObjectsByType<IdolInstance>(FindObjectsSortMode.None);
    }

    public static string CleanNameOfCharacterUI(string raw) // 移除角色 UI 圖片檔名中的多餘字串
    {
        return raw.Replace("UI_character_", "");
    }

    public static string CleanNameOfCharacterObject(string raw) // 移除角色物件名稱中的多餘字串
    {
        return raw.Replace("Character_", "").Replace("2.0", "");
    }
}