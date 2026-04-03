using UnityEngine;

[System.Serializable] // 確保可以被序列化（存檔）
public class ActivityRecord
{
    public Activity selectedActivity; // 活動資訊（從這裡就能知道角色有沒有被指派活動）

    public void SetActivityRecord(Activity selectedActivity)
    {
        this.selectedActivity = selectedActivity;
    }
}
