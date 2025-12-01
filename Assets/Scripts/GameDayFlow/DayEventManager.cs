using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class DayEventManager : MonoBehaviour
{
    public List<DayEvent> allDayEvents; // 用來保存所有的日常事件
    public Queue<DayEvent> eventQueue = new Queue<DayEvent>();//當天需觸發的所有事件
    public DayEvent currentEvent; // 當前正在處理的事件
    private HashSet<string> triggeredEvents = new HashSet<string>(); // 紀錄已觸發事件避免重複觸發
    public int EventedNumberToday =0;
    [Header("紀錄互動事件所需的參數")]
    public bool isAllEventDone=false;
    public bool isWaitingForInteract=false;
    public string interactObjectKey;
    [Header("事件視覺化")]
    public GameObject eventHintPanel;
    public TextMeshProUGUI eventHintText;
    // 初始化當天事件隊列
    public void InitializeDayEvents(int currentDay)
    {
        Debug.Log($"今天是第 {currentDay} 天");
        EventedNumberToday = 0;
        eventQueue.Clear();
        for(int timeIndex=0; timeIndex<200; timeIndex++)
        {
            foreach (var dayEvent in allDayEvents)
            {
                if (dayEvent.TriggerDay == currentDay && dayEvent.TriggerTimeIndex == timeIndex)
                {
                    Debug.Log($"加入事件: {dayEvent.eventId} at time {timeIndex}");
                    eventQueue.Enqueue(dayEvent);
                }
            }
            //WaitAfterDayEndEventStart事件
            if (timeIndex == 100)
            {
                var waitAfterDayEndEventStartEvent = CreateWaitAfterDayEndEventStartEvent();
                eventQueue.Enqueue(waitAfterDayEndEventStartEvent);
            }
        }
        // 最後加上結束一天事件
        var endDayEvent = CreateEndDayEvent();
        eventQueue.Enqueue(endDayEvent);

        isAllEventDone = false;
    }
    private DayEvent CreateWaitAfterDayEndEventStartEvent()
    {
        DayEvent e = ScriptableObject.CreateInstance<DayEvent>();
        e.eventId = "WAIT_AFTER_DAY_END_EVENT_START";
        e.Type = EventType.WaitAfterDayEndEventStart;
        return e;
    }
    private DayEvent CreateEndDayEvent()
    {
        DayEvent e = ScriptableObject.CreateInstance<DayEvent>();
        e.eventId = "END_DAY";
        e.Type = EventType.EndDay;
        return e;
    }

    // 觸發下一個事件
    public void TriggerNextEvent()
    {
        if (eventQueue.Count == 0)
        {
            Debug.Log("No more events to trigger today.");
            isAllEventDone = true;
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
        EventedNumberToday++;
        RunEvent(ev, () => {
            TriggerNextEvent();
        });
    }
    //實際觸發事件的邏輯
    public void RunEvent(DayEvent dayEvent, System.Action onFinish)
    {
        // 根據事件的屬性執行相應的邏輯
        Debug.Log($"Triggering event: {dayEvent.eventId}");
        currentEvent = dayEvent;
        ShowEventHint(dayEvent);
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
        else if (dayEvent.Type == EventType.Teleport)
        {
            // 傳送玩家到指定場景的邏輯
            SceneTransitionManager.Instance.onDialogueFinish = onFinish;
            SceneTransitionManager.Instance.teleportByTargetSceneName(dayEvent.targetSceneName);
        }
        else if (dayEvent.Type == EventType.ShowUIAndWaitExit)
        {
            // 顯示UI並等待玩家關閉的邏輯
            GameObject uiInstance = Instantiate(dayEvent.UIToShow);
            ShowUIAndWaitExit showUIAndWaitExit = uiInstance.GetComponent<ShowUIAndWaitExit>();
            showUIAndWaitExit.StartEvent(() => { 
                onFinish?.Invoke();
                Destroy(uiInstance);
            });
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
        else if (dayEvent.Type== EventType.WaitUntilInteractWithObject)
        {
            // 等待玩家與指定物件互動的邏輯
            isWaitingForInteract = true;
            interactObjectKey = dayEvent.interactableObjectKey;
            TeamManager teamManager = FindAnyObjectByType<TeamManager>();
            if (teamManager != null)
            {
                foreach (var member in teamManager.teamMembers)
                {
                    member.waitInteractionKey= interactObjectKey;
                    member.onInteractionFinish = () =>
                    {
                        isWaitingForInteract = false;
                        onFinish?.Invoke();
                    };
                }
            }
        }
        else if (dayEvent.Type == EventType.WaitForSeconds)
        {
            StartCoroutine(WaitForSec(dayEvent.waitSeconds, onFinish));
        }
        else if (dayEvent.Type == EventType.WaitAfterDayEndEventStart)
        {
            // 電腦結算頁面後
            DayManager.Instance.onDayFinish = onFinish;
        }
        else if (dayEvent.Type == EventType.EndDay)
        {
            // 結束一天
            DayManager.Instance.EndDay();
            // 然後呼叫 onFinish 讓事件管理器知道這個事件結束
            onFinish?.Invoke();
        }
    }
    public void ShowEventHint(DayEvent dayEvent)
    {
        if (dayEvent == null) return;
        if (eventHintPanel == null)
        {
            eventHintPanel = GameObject.FindGameObjectWithTag("EventHint");
            eventHintText = eventHintPanel?.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (dayEvent.isHintEvent)
        {
            eventHintPanel?.gameObject.SetActive(true);
            eventHintText.text = dayEvent.hint;
        }
        else
        {
            eventHintPanel?.gameObject.SetActive(false);
        }
    }
    private IEnumerator WaitForSec(float sec,System.Action onEnd)
    {
        TeamManager teamManager = FindAnyObjectByType<TeamManager>();
        if (teamManager != null)
        {
            teamManager = FindAnyObjectByType<TeamManager>();
            teamManager.teamMembers[
                teamManager.currentLeaderIndex].enabled = false;
        }
        yield return new WaitForSeconds(sec);
        if (teamManager != null)
        {
            teamManager = FindAnyObjectByType<TeamManager>();
            teamManager.teamMembers[
                teamManager.currentLeaderIndex].enabled = true;
        }
        onEnd?.Invoke();
    } 
}
