using UnityEngine;
using TMPro;
using System.Collections;

public class DesktopTutorial : MonoBehaviour
{
    [Header("UI 元件")]
    [SerializeField] private GameObject tutorialBanner; // 懸浮視窗物件
    [SerializeField] private RectTransform bannerRect; // 懸浮視窗的 RectTransform
    [SerializeField] private CanvasGroup canvasGroup; // 用來控制透明度淡入淡出
    [SerializeField] private TextMeshProUGUI tutorialText; // 提示文字元件
    //-----------------------------------------------------------------//
    [Header("動態時間設定")]
    [SerializeField] private float lagDuration = 1f; // 打開訓練 UI 後的延遲開啟時間
    [SerializeField] private float moveDuration = 1f; // 浮上來＆消下去的花費時間（秒）
    [SerializeField] private float stayDuration = 5f; // 消失動態開始前的文字停留時間
    //-----------------------------------------------------------------//
    [Header("座標位置設定")]
    [SerializeField] private float hiddenY = -435f; // 隱藏時的位置（起始）
    [SerializeField] private float visibleY = -405f; // 顯示時的位置（終點）
    private Coroutine activeAnimation; // 用來記錄與管理當前的動畫協程

    void Start()
    {
        SetAppUI.OnAppWithTutorialOpened += ShowAndCompleteTutorial; // 訂閱 App 開啟事件，觸發提示橫幅的顯示與完成流程
        CloseWindow.OnAppWithTutorialClosed += Close; // 訂閱 App 關閉事件，觸發提示橫幅的隱藏

        tutorialBanner.SetActive(false); // 預設隱藏提示橫幅
    }

    void OnDestroy()
    {
        SetAppUI.OnAppWithTutorialOpened -= ShowAndCompleteTutorial; // 取消訂閱事件
        CloseWindow.OnAppWithTutorialClosed -= Close;
    }

    public void ShowAndCompleteTutorial(string contentText)
    {
        if (activeAnimation != null) StopCoroutine(activeAnimation);

        SetText(contentText);
        activeAnimation = StartCoroutine(FullMotionSequence());
    }

    private void Close()
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


    private IEnumerator FullMotionSequence() // 結合出現與消失的協程
    {
        if (lagDuration > 0f) yield return new WaitForSeconds(lagDuration);

        tutorialBanner.SetActive(true);
        yield return AnimateBanner(hiddenY, visibleY, true); // 完整播放向上浮的動態

        // 讓提示在畫面上硬性停留（靜態顯示）
        yield return new WaitForSeconds(stayDuration);

        // 播放向下滑動消除的動畫，並等待它播完
        yield return AnimateBanner(visibleY, hiddenY, false);

        // 結束所有動態後徹底關閉
        tutorialBanner.SetActive(false);
        activeAnimation = null;
    }

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
