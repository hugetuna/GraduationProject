using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 放在聊天室視窗根部以生成不同分類 ScrollView 中的用戶們 */
public class UserUIGenerator : MonoBehaviour
{
    [Header("用戶資料")]
    // 目前沒有任何地方能獲取用戶清單之類的
    public List<User> userList = new(); // 儲存用戶資訊的清單
    private List<UserRuntime> userRuntimes = new();
    //-----------------------------------------------------------------//
    public GameObject userPrefab; // 用於生成用戶項目的預製件
    public List<Transform> userContent = new(); // 用於放置生成的用戶物件的容器

    void Start()
    {
        CreateNewRuntimes();

        // 從無處獲取用戶清單
        foreach (UserRuntime userRuntime in userRuntimes) // 按清單生成初始的用戶項目
        {
            // 生成用戶並分類
            GameObject userObject = null;
            User user = userRuntime.user;
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
            button.GetComponent<SetUserUI>().Initialize(userRuntime); // 設定用戶項目的 UI 資料

            // 全部分類裡也要放（記得初始化）
            GameObject obj = Instantiate(userPrefab, userContent[0]);
            GameObject btn = obj.transform.Find("Button").gameObject; // Wrapper + "Button"
            btn.GetComponent<SetUserUI>().Initialize(userRuntime); // 設定用戶項目的 UI 資料
        }
    }

    private void CreateNewRuntimes()
    {
        var savedUserList = GameManager.Instance.chatSaveData.users;

        // 先把目前存檔裡所有的 ID 撈出來放在 HashSet (提升查詢速度)
        HashSet<string> existingIds = new();
        foreach (var user in savedUserList) existingIds.Add(user.id);

        int totalDays = DayManager.Instance.date + DayManager.Instance.chapter * 3; // 僅適用於新手教學＆第一章


        foreach (User user in userList)
        {
            if (!existingIds.Contains(user.id))
            {
                // 新用戶邏輯：建立新的 UserRuntime 並初始化
                UserRuntime newRuntime = new() { user = user, id = user.id };
                newRuntime.ReloadState(); // 初始化 Ink 故事

                userRuntimes.Add(newRuntime); // 加入動態清單
                GameManager.Instance.SaveChatData(newRuntime); // 自動同步存檔
            }
            else
            {
                // 存檔中已存在用戶邏輯：重新連線 SO 與 Story
                UserRuntime runtime = savedUserList.Find(r => r.id == user.id);
                runtime.user = user;

                // 只有 ID 為 1 的用戶（預約老師）才執行每日跨天重置檢查
                // 務必放在 ReloadState 之前，這樣重置完乾淨的狀態才能正確被載入
                if (runtime.id == "1")
                {
                    runtime.DailyReset(totalDays);
                }
                runtime.ReloadState(); // 初始化 Ink 故事（會自動載入存檔）

                userRuntimes.Add(runtime); // 加入動態清單
            }
        }
    }
}
