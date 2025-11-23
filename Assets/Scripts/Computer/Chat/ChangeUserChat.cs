using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 掛在聊天室視窗的 Rightside 聊天室上 */
public class ChangeUserChat : MonoBehaviour
{
    [Tooltip("聊天室標題文字")]
    [SerializeField] private TextMeshProUGUI chatNameText;
    //-----------------------------------------------------------------//
    [SerializeField] private ChatSaveData chatSaveData; // User + UserRuntime 紀錄
    private static User currentUser = null; // 目前正在聊天的用戶（靜態資料）

    void Start()
    {
        SetUserUI.OnUserContentClicked += ChangeUser; // 訂閱用戶項目被點擊事件
    }

    void OnDestroy()
    {
        SetUserUI.OnUserContentClicked -= ChangeUser; // 取消訂閱用戶項目被點擊事件
    }

    private void ChangeUser(User user)
    {
        currentUser = user;
        Debug.Log($"切換到與 {user.userName} 的聊天室");

        // 切換聊天室的用戶 UI
        chatNameText.text = user.userName;
        chatNameText.ForceMeshUpdate();
        ChatBubbleManager.Instance.SetUserBubbleIcon(user.userIcon);

        // 聊天內容與對應 UI 的切換
        if (!chatSaveData.users.TryGetValue(user, out var userRuntime))
        {
            userRuntime = new UserRuntime() { user = user };
            chatSaveData.users[user] = userRuntime;
        }

        // 備份 chatSaveData 的變動
        GameManager.Instance.SaveChatData(chatSaveData); 

        // 切換用戶的前置作業完成，正式開始聊天
        ChatRoomManager.Instance.StartChatting(userRuntime);
    }
}
