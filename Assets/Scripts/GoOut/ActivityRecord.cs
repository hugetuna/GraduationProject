using UnityEngine;

[System.Serializable] // 確保可以被序列化（存檔）
public class ActivityRecord
{
    public Activity selectedActivity; // 活動資訊（從這裡就能知道角色有沒有被指派活動）
    public int realMoneyGain; // 商演實際上所賺的錢（結算用）

    public void SetActivityRecord(Activity selectedActivity, int realMoneyGain = -1)
    {
        this.selectedActivity = selectedActivity;
        if (realMoneyGain != -1) this.realMoneyGain = realMoneyGain; // 若引數為 -1 代表不做更新
    }
}
