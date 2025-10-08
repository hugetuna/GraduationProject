using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在聊天室本身的 ScrollView 上（Singleton）*/
public class ChatBubbleManager : MonoBehaviour
{
    public static ChatBubbleManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [Header("訊息泡泡 UI ")]
    [SerializeField] private GameObject userBubblePrefab; // 訊息泡泡預製件（用戶）
    [SerializeField] private GameObject playerBubblePrefab; // 訊息泡泡預製件（玩家）
    //-----------------------------------------------------------------//
    [Header("聊天室排版")]
    [SerializeField] private ScrollRect scrollRect; // 該物件的 ScrollRect 組件
    [SerializeField] private RectTransform content; // 該物件底下的 Content
    [SerializeField] private float maxTextWidth = 200f; // 訊息泡泡文字之最大寬度
    [SerializeField] private float heightPerRow = 12f; // 每增加一行文字，泡泡高度增加的數值
    private float baseHeight; // 泡泡的基礎高度

    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    void Start()
    {
        // 取得泡泡的基礎高度（userBubblePrefab 和 playerBubblePrefab 一樣）
        baseHeight = userBubblePrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void AddBubble(string message, bool isPlayer)
    {
        // 選擇泡泡 prefab
        GameObject bubblePrefab = isPlayer ? playerBubblePrefab : userBubblePrefab;
        GameObject bubble = Instantiate(bubblePrefab, content);

        // 設定文字內容
        TextMeshProUGUI messageText = bubble.GetComponentInChildren<TextMeshProUGUI>();
        messageText.text = message;

        // 計算並設定泡泡高度
        int rowNum = (int)(messageText.preferredWidth / maxTextWidth) + 1; // 計算文字的行數，預設為 1 行
        float bubbleHeight = baseHeight + (rowNum - 1) * heightPerRow; // 計算泡泡高度
        RectTransform bubbleRect = bubble.GetComponent<RectTransform>();
        bubbleRect.sizeDelta = new Vector2(bubbleRect.sizeDelta.x, bubbleHeight);

        // 限制泡泡最大寬度
        LayoutElement layout = bubble.GetComponentInChildren<LayoutElement>();
        if (messageText.preferredWidth > maxTextWidth)
        {
            layout.preferredWidth = maxTextWidth;
        }
        else
        {
            layout.preferredWidth = -1; // -1 表示自動大小
        }

        // 強制更新 Layout，避免捲動時尺寸錯亂
        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);

        // 自動捲到最底
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
