using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 放在聊天室視窗根部以生成不同分類 ScrollView 中的用戶們 */
public class UserUIGenerator : MonoBehaviour
{
    [Header("用戶資料")]
    // 目前沒有任何地方能獲取用戶清單之類的
    public List<User> userList = new(); // 儲存用戶資訊的清單
    //-----------------------------------------------------------------//
    public GameObject userPrefab; // 用於生成用戶項目的預製件
    public List<Transform> userContent = new(); // 用於放置生成的用戶物件的容器

    void Start()
    {
        // 從無處獲取用戶清單
        foreach (User user in userList) // 按清單生成初始的用戶項目
        {
            // 生成用戶並分類
            GameObject userObject = null;
            if (user.userType == UserType.Friend)
            {
                userObject = Instantiate(userPrefab, userContent[1]); // "Wrapper" + Button
            }
            else if (user.userType == UserType.Teacher)
            {
                userObject = Instantiate(userPrefab, userContent[2]); // "Wrapper" + Button
            }
            else if (user.userType == UserType.Capital)
            {
                userObject = Instantiate(userPrefab, userContent[3]); // "Wrapper" + Button
            }
            if (userObject == null)
            {
                Debug.Log("用戶項目生成失敗！");
                continue;
            }

            GameObject button = userObject.transform.Find("Button").gameObject; // Wrapper + "Button"
            button.GetComponent<SetUserUI>().Initialize(user); // 設定用戶項目的 UI 資料

            // 全部分類裡也要放（記得初始化）
            GameObject obj = Instantiate(userPrefab, userContent[0]);
            GameObject btn = obj.transform.Find("Button").gameObject; // Wrapper + "Button"
            btn.GetComponent<SetUserUI>().Initialize(user); // 設定用戶項目的 UI 資料
            }
    }
}
