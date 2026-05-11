using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "DayFlow/Day Event Set")]
public class DayEventSet : ScriptableObject
{
    public int chapterIndex; // 用來標識這是第幾章的事件組
    public int dayIndex; // 用來標識這是第幾天的事件組
    public List<DayEvent> todayEvents; // 用來保存所有的日常事件
}
