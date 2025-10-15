using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在聊天室視窗的 Rightside 聊天室上 */
public class ChangeUserChat : MonoBehaviour
{
    [Tooltip("聊天室標題文字")]
    [SerializeField] private TextMeshProUGUI chatNameText;
    private Dictionary<User, UserRuntime> userRuntimeDict = new();
    private static User currentUser = null; // 目前正在聊天的用戶（靜態資料）

    void Start()
    {
        SetUserUI.OnUserContentClicked += ChangeChatUser; // 訂閱用戶項目被點擊事件
    }

    void OnDestroy()
    {
        SetUserUI.OnUserContentClicked -= ChangeChatUser; // 取消訂閱用戶項目被點擊事件
    }

    private void ChangeChatUser(User user)
    {
        currentUser = user;
        Debug.Log($"切換到與 {user.userName} 的聊天室");

        // 切換聊天室的用戶 UI
        chatNameText.text = user.userName;
        chatNameText.ForceMeshUpdate();

        // 聊天內容與對應 UI 的切換
        if (!userRuntimeDict.TryGetValue(user, out var userRuntime))
        {
            userRuntime = new UserRuntime() { user = user };
            userRuntimeDict[user] = userRuntime;
        }
        ChatRoomManager.Instance.StartChatting(userRuntime);
    }
}
