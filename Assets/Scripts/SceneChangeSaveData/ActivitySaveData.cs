using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ActivitySaveData
{
    public List<Activity> activities = new(); // 沒有動態資料所以直接存就好

    // public void SaveActivityData(Activity activity)
    // {
    //     activities.Add(activity);
    // }

    public void RemoveActivity(Activity activity)
    {
        activities.Remove(activity);
    }

    public Activity GetTodayActivity() // 獲取今日預約的商演（不含公演）
    {
        Activity dayActivities = null;
        foreach (Activity activity in activities)
        {
            if (activity.day == DayManager.Instance.date && !activity.description.Contains("公演"))
            {
                dayActivities = activity; // 一天最多只會有一個商演
            }
        }

        return dayActivities;
    }

    public bool IsActivityAppointed(Activity activity)
    {
        return activities.Contains(activity);
    }

    public void CleanGoneActivities() // 清除已結束的商演資料（預計在每日結算時進行）
    {
        activities.RemoveAll(activity => activity.day < DayManager.Instance.date);
    }
}
