using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 動態管理 User ScriptableObject 難以管理的資料 */
public class UserRuntime
{
    public User user; // 指向 User ScriptableObject 的參考
    public int unreadCount; // 未讀訊息數
}
