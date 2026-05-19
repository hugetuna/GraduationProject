using System.Collections;
using UnityEngine;
using TMPro;

public class ActivityTutorial : MonoBehaviour
{
    [Header("UI 元件")]
    [SerializeField] private GameObject tutorialBanner; // 懸浮視窗物件
    [SerializeField] private RectTransform bannerRect; // 懸浮視窗的 RectTransform
    [SerializeField] private CanvasGroup canvasGroup; // 用來控制透明度淡入淡出
    [SerializeField] private TextMeshProUGUI tutorialText; // 提示文字元件
    //-----------------------------------------------------------------//
    [Header("動態時間設定")]
    [SerializeField] private float lagDuration = 1f; // 打開外出介面後的延遲開啟時間
    [SerializeField] private float moveDuration = 0.6f; // 浮上來＆消下去的花費時間（秒）
    // [SerializeField] private float stayDuration = 1.2f; // 消失動態開始前的文字停留時間
    //-----------------------------------------------------------------//
    [Header("座標位置設定")]
    [SerializeField] private float hiddenY = -496f; // 隱藏時的位置（起始）
    [SerializeField] private float visibleY = -466f; // 顯示時的位置（終點）
    private Coroutine activeAnimation; // 用來記錄與管理當前的動畫協程
    //-----------------------------------------------------------------//
    private bool isOnceShown = false; // 確保提示在出現一次後不再播放出場動態

    void Start()
    {
        tutorialBanner.SetActive(false); // 預設隱藏提示橫幅
    }

    public void ShowTutorial()
    {
        SetText("今天沒有可用的商演，之後可以到辦公室電腦預約");
        Show(lagDuration);
        // CompleteAndHide(); // 等待 stayDuration 秒後自動消失
    }

    public void Close()
    {
        // 關閉所有動態
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }

        tutorialBanner.SetActive(false);

        // 將移動到一半的座標歸位
        if (bannerRect != null)
        {
            Vector2 pos = bannerRect.anchoredPosition;
            pos.y = hiddenY;
            bannerRect.anchoredPosition = pos;
        }
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public void SetText(string contentText)
    {
        if (tutorialText != null)
        {
            tutorialText.text = contentText;
        }
    }

    //-----------------------------------------------------------------//

    public void Show(float delay = 0f)
    {
        // 如果前一個動畫還在跑，先強行停止
        if (activeAnimation != null) StopCoroutine(activeAnimation);

        // 開始跑包含延遲的完整顯示序列
        if (!isOnceShown)
        {
            isOnceShown = true;
            activeAnimation = StartCoroutine(ShowSequence(delay));
        }
        else
        {
            // 已經出現過一次了，直接顯示內容但不跑動態
            tutorialBanner.SetActive(true);

            // 確保位置在可見狀態
            Vector2 pos = bannerRect.anchoredPosition;
            pos.y = visibleY;
            bannerRect.anchoredPosition = pos;
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }
    }

    private IEnumerator ShowSequence(float delay) // 處理「先等待再動態」的流程
    {
        // 1. 如果有設定延遲時間就先等待
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // 2. 時間到了才正式啟用物件
        tutorialBanner.SetActive(true);

        // 3. 完整播放向上浮的動態
        yield return AnimateBanner(hiddenY, visibleY, true);
        activeAnimation = null;
    }

    // public void CompleteAndHide() // 動作完成後停留一下，再開始「緩慢向下滑動消除」
    // {
    //     if (activeAnimation != null) StopCoroutine(activeAnimation);

    //     // 開始跑成功停留與下滑的序列
    //     activeAnimation = StartCoroutine(HideSequence());
    // }

    // private IEnumerator HideSequence() // 停留與下滑協程
    // {
    //     // 1. 在畫面上硬性停留一段時間，讓玩家看清楚成功提示
    //     yield return new WaitForSeconds(stayDuration);

    //     // 2. 沿著原路徑滑下去
    //     yield return AnimateBanner(visibleY, hiddenY, false);

    //     // 3. 徹底關閉物件
    //     tutorialBanner.SetActive(false);
    //     activeAnimation = null;
    // }

    private IEnumerator AnimateBanner(float startY, float endY, bool isDisplaying) // 處理純位移與透明度變化的核心協程
    {
        float elapsedTime = 0f;
        Vector2 currentPosition = bannerRect.anchoredPosition;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // 使用 Mathf.SmoothStep 讓動態漸進式減速（Ease-In-Ease-Out）
            float smoothedT = Mathf.SmoothStep(0f, 1f, t);

            // 改變 Y 軸座標
            currentPosition.y = Mathf.Lerp(startY, endY, smoothedT);
            bannerRect.anchoredPosition = currentPosition;

            // 如果有掛載 CanvasGroup，順便處理透明度變化
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(isDisplaying ? 0f : 1f, isDisplaying ? 1f : 0f, smoothedT);
            }

            yield return null;
        }

        // 確保最終數值精準賦值
        currentPosition.y = endY;
        bannerRect.anchoredPosition = currentPosition;
        if (canvasGroup != null) canvasGroup.alpha = isDisplaying ? 1f : 0f;
    }
}
