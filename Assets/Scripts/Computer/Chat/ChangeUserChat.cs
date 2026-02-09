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
    private static UserRuntime currentUserRuntime = null; // 目前正在聊天的用戶
    //-----------------------------------------------------------------//
    private ChatRoomController chatRoomController;

    void Start()
    {
        SetUserUI.OnUserContentClicked += ChangeUser; // 訂閱用戶項目被點擊事件
        chatRoomController = GetComponent<ChatRoomController>();
    }

    void OnDestroy()
    {
        SetUserUI.OnUserContentClicked -= ChangeUser; // 取消訂閱用戶項目被點擊事件
    }

    private void ChangeUser(UserRuntime userRuntime)
    {
        currentUserRuntime = userRuntime;
        var currentUser = currentUserRuntime.user;
        Debug.Log($"切換到與 {currentUser.userName} 的聊天室");

        // 切換聊天室的用戶 UI
        chatNameText.text = currentUser.userName;
        chatNameText.ForceMeshUpdate();
        ChatBubbleManager.Instance.SetUserBubbleIcon(currentUser.userIcon);

        // 切換聊天室用戶資料，正式開始聊天
        chatRoomController.StartChatting(currentUserRuntime);
    }
}
