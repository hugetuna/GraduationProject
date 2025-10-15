using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

/* 動態管理 User ScriptableObject 難以管理的資料 */
public class UserRuntime
{
    public User user; // 指向 User ScriptableObject 的參考
    public string savedInkState;  // Ink 進度 JSON
    public Story story; // Ink 的故事實例
    public List<(string text, bool isPlayer)> chatHistory = new(); // 聊天記錄
    // public int unreadCount; // 未讀訊息數

    public void Initialize()
    {
        story = new Story(user.inkJSONAsset.text);
        if (!string.IsNullOrEmpty(savedInkState))
        {
            story.state.LoadJson(savedInkState);
            Debug.Log($"{user.name} 的對話狀態已載入");
        }
    }

    public void SaveState()
    {
        if (story != null)
        {
            savedInkState = story.state.ToJson();
            Debug.Log($"{user.name} 的對話進度已儲存");
        }
    }
}
