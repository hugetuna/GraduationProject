using System.Collections.Generic;

/* 用來儲存所有用戶聊天資訊（與 GameManager 連接以跨場景保存資料） */
[System.Serializable]
public class ChatSaveData
{
    public List<UserRuntime> users = new();
}
