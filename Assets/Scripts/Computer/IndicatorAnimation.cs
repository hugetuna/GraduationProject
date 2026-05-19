using UnityEngine;

public class IndicatorAnimation : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 3f;  // 漂浮的速度
    [SerializeField] private float floatAmount = 15f; // 上下飄移的幅度 (像素)

    private RectTransform rectTransform;
    private float startY;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y; // 記住遊戲開始時，這個三角形「原本該在的位置」
    }

    void Update()
    {
        // 使用 Mathf.Sin 函數算出上下平滑正弦波
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = newY;
        rectTransform.anchoredPosition = pos;
    }

    void OnEnable() // 當物件被關閉又打開時，重置回起點，避免累加誤差
    {
        if (rectTransform != null)
        {
            Vector2 pos = rectTransform.anchoredPosition;
            pos.y = startY;
            rectTransform.anchoredPosition = pos;
        }
    }
}
