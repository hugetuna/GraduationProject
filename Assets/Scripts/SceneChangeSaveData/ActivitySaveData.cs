using System;
using System.Collections.Generic;

[Serializable]
public class ActivitySaveData
{
    public List<ActivityAppointment> activities = new(); // 沒有動態資料所以直接存就好

    // public void SaveActivityData(Activity activity)
    // {
    //     activities.Add(activity);
    // }

    public void RemoveActivity(Activity activity, int appointDay)
    {
        activities.RemoveAll(a => a.activity == activity && a.appointDay == appointDay);
    }

    public Activity GetTodayActivity() // 獲取今日預約的商演（不含公演）
    {
        Activity dayActivities = null;
        foreach (ActivityAppointment appoint in activities)
        {
            if (appoint.appointDay == DayManager.Instance.day && !appoint.activity.description.Contains("公演"))
            {
                dayActivities = appoint.activity; // 一天最多只會有一個商演
            }
        }

        return dayActivities;
    }

    public bool IsActivityAppointed(Activity activity)
    {
        return activities.Exists(a => a.activity == activity);
    }

    public int GetAppointDay(Activity activity)
    {
        ActivityAppointment appoint = activities.Find(a => a.activity == activity);
        return appoint != null ? appoint.appointDay : -1; // 若未找到則返回 -1
    }

    public void CleanGoneActivities() // 清除已結束的商演資料（預計在每日結算時進行）
    {
        activities.RemoveAll(activity => activity.appointDay < DayManager.Instance.day);
    }
}

[Serializable]
public class ActivityAppointment
{
    public Activity activity; // 沒有動態資料所以直接存就好
    public int appointDay; // 預約日期（活動日期存在 Activity 裡面）
}
