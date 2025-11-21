using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayEventManager : MonoBehaviour
{
    public List<DayEvent> allDayEvents; // 用來保存所有的日常事件
    private Queue<DayEvent> eventQueue = new Queue<DayEvent>();//當天需觸發的所有事件
    private HashSet<string> triggeredEvents = new HashSet<string>(); // 紀錄已觸發事件避免重複觸發
    // 初始化當天事件隊列
    public void InitializeDayEvents(int currentDay)
    {
        Debug.Log($"今天是第 {currentDay} 天");
        eventQueue.Clear();
        foreach (var dayEvent in allDayEvents)
        {
            if (dayEvent.TriggerDay == currentDay)
            {
                eventQueue.Enqueue(dayEvent);
            }
        }
    }
    // 觸發下一個事件
    public void TriggerNextEvent()
    {
        if (eventQueue.Count == 0)
        {
            Debug.Log("No more events to trigger today.");
            return;
        }
        var ev = eventQueue.Dequeue();
        // 檢查是否已經觸發過且只觸發一次
        if (ev.onlyTriggerOnce && triggeredEvents.Contains(ev.eventId))
        {
            Debug.Log($"Event {ev.eventId} has already been triggered. Skipping.");
            TriggerNextEvent(); // 繼續觸發下一個事件
            return;
        }
        RunEvent(ev, () => {
            TriggerNextEvent();
        });
    }
    //實際觸發事件的邏輯
    public void RunEvent(DayEvent dayEvent, System.Action onFinish)
    {
        // 根據事件的屬性執行相應的邏輯
        Debug.Log($"Triggering event: {dayEvent.eventId}");
        if (dayEvent.Type== EventType.MainWorld)
        {
            GameManager.Instance.SaveInkJSONAssetData(dayEvent.DialogueWhenTrigger);
            DialogueManager.Instance.onDialogueFinish = onFinish;
            DialogueManager.Instance.DialogueStart();
        }
        else if (dayEvent.Type== EventType.Dialogue)
        {
            // 在對話中觸發事件的邏輯
            GameManager.Instance.SaveInkJSONAssetData(dayEvent.DialogueWhenTrigger);
            DialogueManager.Instance.onDialogueFinish = onFinish;
            SceneTransitionManager.Instance.teleportByTargetSceneName("Dialogue Scene");
        }
        else if (dayEvent.Type== EventType.WaitUntilSceneChange)
        {
            // 等待場景切換的邏輯
            SceneTransitionManager.Instance.onDialogueFinish = onFinish;
            SceneTransitionManager.Instance.waitSceneName= dayEvent.targetSceneName;
        }
        else if (dayEvent.Type== EventType.WaitUntilPlayerPosition)
        {
            // 等待玩家移動到指定位置的邏輯
            var waitEventObj = new GameObject("WaitPlayerEnterAreaEvent");
            var waitEvent = waitEventObj.AddComponent<WaitPlayerEnterAreaEvent>();
            waitEvent.StartEvent(dayEvent.targetPlayerPositionMin, dayEvent.targetPlayerPositionMax, () =>
            {
                onFinish?.Invoke();
                Destroy(waitEventObj);
            });
        }
    }
}
