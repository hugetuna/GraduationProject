using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(fileName = "ChatSaveData", menuName = "Computer/ChatSaveData")]

/* 用來儲存所有用戶聊天資訊（與 GameManager 連接以跨場景保存資料） */
public class ChatSaveData : ScriptableObject
{
    public Dictionary<User, UserRuntime> users = new();

}
