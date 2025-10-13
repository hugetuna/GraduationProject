using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在聊天室視窗的 Rightside 聊天室上 */
public class ChooseUserRespond : MonoBehaviour
{
    [Tooltip("聊天室標題文字")]
    [SerializeField] private TextMeshProUGUI chatNameText;

    [Tooltip("回應按鈕們（目前有三個）")]
    // [SerializeField] private List<Button> respondButtons;
    private static User currentUser = null; // 目前正在聊天的用戶

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

        // 聊天內容的切換之後再實作
    }

    public static User GetCurrentUser()
    {
        return currentUser;
    }

    // public void TurnOnRespondButtons()
    // {
    //     foreach (Button button in respondButtons)
    //     {
    //         button.interactable = true; // 開啟回應按鈕
    //     }
    // }

    // public void TurnOffRespondButtons()
    // {
    //     foreach (Button button in respondButtons)
    //     {
    //         button.interactable = false; // 關閉回應按鈕
    //     }
    // }
}
