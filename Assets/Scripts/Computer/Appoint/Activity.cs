using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "Activity", menuName = "Computer/Activity")]
public class Activity : ScriptableObject
{
    public string activityName; // 活動名稱
    public string description; // 活動描述
    public Sprite activityImage; // 活動圖片
    public TicketColor ticketColorId; // 活動票券樣式，共三種
    public int day; // 活動日期（對應 DayManager 的天數，從 1 開始）
    public int vigourCost; // 活動耗體
    public int fee; // 活動費用
    public int MoneyGain; // 活動的金錢收益
    public StageAttribute stageAttribute; // 活動對應的舞台屬性
}
