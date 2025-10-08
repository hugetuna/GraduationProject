using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Message
{
    public string senderId; // 發送者 ID（與 User 的 id 對應）
    public string content; // 訊息的文字內容
    public DateTime time; // 傳送時間
    public bool isRead; // 是否已讀
}
