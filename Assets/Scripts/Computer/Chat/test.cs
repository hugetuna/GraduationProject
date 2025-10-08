using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 很隨便地掛在聊天室視窗 Rightside 的 Center 上 */
public class test : MonoBehaviour
{
    public ChatBubbleManager chatBubbleManager;
    private int count = 0;
    private List<string> messages = new();
    private List<bool> isPlayer = new();

    void Start()
    {
        chatBubbleManager = ChatBubbleManager.Instance;

        // 測試用訊息
        messages.Add("你好");
        messages.Add("初次見面");
        messages.Add("你好啊");
        messages.Add("很高興認識你");
        messages.Add("我們要不要找時間一起吃頓飯？");
        messages.Add("沒問題，你想吃什麼？");
        messages.Add("這個嘛……交給你選吧，你喜歡吃什麼？");
        isPlayer.Add(false);
        isPlayer.Add(false);
        isPlayer.Add(true);
        isPlayer.Add(true);
        isPlayer.Add(false);
        isPlayer.Add(true);
        isPlayer.Add(false);

        // 選項測試

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            chatBubbleManager.AddBubble(messages[count], isPlayer[count]);
            count++;
            if (count >= messages.Count)
            {
                count = 0;
            }
        }
    }
}
