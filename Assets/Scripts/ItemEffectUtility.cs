using System.Collections.Generic;
using System.Linq;

public class ItemEffectInfo
{
    public string id;
    public string displayName;
}

public class ItemEffectUtility
{
    /* 訓練效果專區（會因角色有所區別） */
    private static Dictionary<IdolWho, List<ItemEffectInfo>> trainingEffects = new(); // 儲存各角色目前啟用的訓練加成效果 ID 和 displayName

    public static void SaveTrainingEffect(IdolWho idol, ItemEffectInfo effectInfo)
    {
        if (!trainingEffects.ContainsKey(idol))
        {
            trainingEffects[idol] = new List<ItemEffectInfo>();
        }
        trainingEffects[idol].Add(effectInfo);
    }

    public static List<string> GetTrainingEffectDisplayNames(IdolWho idol, TrainingType trainingType)
    {
        if (!trainingEffects.ContainsKey(idol))
        {
            return new List<string>(); // 這個角色沒有任何訓練加成效果，回傳空清單
        }

        List<ItemEffectInfo> effectsForIdol = trainingEffects[idol];
        // if(trainingType == TrainingType.None)
        // {
        //     return effectsForIdol.Select(e => e.displayName).ToList(); // 回傳這個角色所有訓練加成效果的名稱
        // }
        string prefix = trainingType.ToString().ToLower() + "_"; // 例如 "dance_"
        return effectsForIdol
            .Where(e => e.id.StartsWith(prefix)) // 篩選出該角色符合訓練類型的效果
            .Select(e => e.displayName) // 取出名稱
            .ToList(); // 整理成清單並回傳
    }

    public static void ResetTrainingEffects()
    {
        trainingEffects.Clear(); // 跟偶像的訓練加成一起在每天結束後重置
    }

    //-----------------------------------------------------------------//

    /* 全域效果專區（不會因角色有所區別）*/
    private static List<ItemEffectInfo> globalEffects = new(); // 儲存目前啟用的全域效果 ID 和 displayName
    public static void SaveGlobalEffect(ItemEffectInfo effectInfo)
    {
        if (!globalEffects.Any(e => e.id == effectInfo.id))
        {
            globalEffects.Add(effectInfo);
        }
    }

    public static List<string> GetGlobalEffectDisplayNames()
    {
        return globalEffects.Select(e => e.displayName).ToList();
    }

    public static void ResetGlobalEffects()
    {
        globalEffects.Clear(); // 跟一般加成一起在每天結束後重置
    }

    //-----------------------------------------------------------------//

    public static void ResetAllEffects()
    {
        ResetTrainingEffects();
        ResetGlobalEffects();
    }
}
