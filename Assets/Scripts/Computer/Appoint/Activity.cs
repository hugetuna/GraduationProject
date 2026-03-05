using UnityEngine;

[CreateAssetMenu(fileName = "Activity", menuName = "Computer/Activity")]
public class Activity : ScriptableObject
{
    public string activityName; // 活動名稱
    public string description; // 活動描述
    public Sprite activityImage; // 活動圖片
    public string date; // 活動日期（格式：YYYY.MM.DD）
    public int vigourCost; // 活動耗體
    public int fee; // 活動費用
    public int MoneyGain; // 活動的金錢收益
}
