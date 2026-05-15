using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using NUnit.Framework;

/* 動態管理 User ScriptableObject 難以管理的資料 */
[System.Serializable]
public class UserRuntime
{
    public string id; // 用戶 ID（方便查找）
    [System.NonSerialized] public User user; // 指向 User ScriptableObject 的參考
    [System.NonSerialized] public Story story; // Ink 的故事實例
    public string savedInkState;  // Ink 進度 JSON
    public List<ChatHistory> chatHistory = new(); // 聊天紀錄
    //-----------------------------------------------------------------//
    public bool isUnread = true;  // 是否有未讀訊息
    public string lastMessageText = ""; // 最後一則訊息的內容
    public int lastMessageDay = -1; // 紀錄最後一則訊息是第幾天

    public void ReloadState()
    {
        if (user == null || user.inkJSONAsset == null)
        {
            Debug.LogError($"User SO 或 Ink 資源，無法初始化！");
            return;
        }

        // 故事初始化
        if (story == null)
        {
            // 若尚未有故事實例，則重開一個新的
            story = new Story(user.inkJSONAsset.text);
            // Debug.Log($"初始化 {user.name} 的 Ink 故事");
        }

        if (!string.IsNullOrEmpty(savedInkState)) // 嘗試讀取存檔
        {
            // 有存檔的話就載入，覆蓋原本 story 的狀態
            story.state.LoadJson(savedInkState);
            Debug.Log($"{user.name} 的對話狀態已載入");
        }

        // 若無存檔，預設為當前日期
        if(lastMessageDay == -1) lastMessageDay = DayManager.Instance.day;
    }

    public void AddToChatHistory(string text, bool isPlayer)
    {
        chatHistory.Add(new ChatHistory() { text = text, isPlayer = isPlayer });
    }

    public void SaveState()
    {
        if (story != null)
        {
            savedInkState = story.state.ToJson();
            Debug.Log($"{user.name} 的對話進度已儲存");
        }

        // // 目前用戶不會在介面以外傳訊息，所以運作邏輯會跟一般的聊天室不太一樣
        // var count = chatHistory.Count;
        // if (count > 0)
        // {
        //     lastMessageText = chatHistory[count - 1].text;
        //     lastMessageDay = DayManager.Instance.day;
        //     if (lastMessageText == "＊此次對話已結束＊")
        //     {
        //         isUnread = false; // 對話結束後不再視為未讀
        //     }
        //     else isUnread = true;
        // }
    }
}

[System.Serializable]
public class ChatHistory
{
    public string text;
    public bool isPlayer;
}
