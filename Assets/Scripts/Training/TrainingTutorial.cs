using System.Collections;
using TMPro;
using UnityEngine;

/* 掛在 UIManager 上 */
public class TrainingTutorial : MonoBehaviour
{
    [Header("UI 元件")]
    [SerializeField] private GameObject tutorialBanner; // 懸浮視窗物件
    [SerializeField] private RectTransform bannerRect; // 懸浮視窗的 RectTransform
    [SerializeField] private CanvasGroup canvasGroup; // 用來控制透明度淡入淡出
    [SerializeField] private TextMeshProUGUI tutorialText; // 提示文字元件
    //-----------------------------------------------------------------//
    [Header("動態時間設定")]
    [SerializeField] private float lagDuration = 1f; // 打開訓練 UI 後的延遲開啟時間
    [SerializeField] private float moveDuration = 0.6f; // 浮上來＆消下去的花費時間（秒）
    [SerializeField] private float stayDuration = 1.2f; // 動作成功後，成功的提示文字之停留時間
    //-----------------------------------------------------------------//
    [Header("座標位置設定")]
    [SerializeField] private float hiddenY = -556f; // 隱藏時的位置（起始）
    [SerializeField] private float visibleY = -466f; // 顯示時的位置（終點）
    private Coroutine activeAnimation; // 用來記錄與管理當前的動畫協程
    //-----------------------------------------------------------------//
    private bool isOnceShown = false; // 確保提示在出現一次後不再播放出場動態

    void Start()
    {
        TrainingUIHandler.OnTrainingUIOpened += CallShow; // 訂閱訓練 UI 開啟事件，觸發提示橫幅的顯示  
        TrainingVigourBar.OnTrainingTutorialDone += CallCompleteAndHide; // 訂閱訓練室新手教學完成事件，觸發提示橫幅的成功提示與消除
        TrainingUIHandler.OnTrainingUIClosed += Close; // 訂閱訓練 UI 關閉事件，觸發提示橫幅的隱藏

        tutorialBanner.SetActive(false); // 預設隱藏提示橫幅
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIOpened -= CallShow; // 取消訂閱事件
        TrainingVigourBar.OnTrainingTutorialDone -= CallCompleteAndHide;
        TrainingUIHandler.OnTrainingUIClosed -= Close;
    }

    private void CallShow()
    {
        Show("將指定角色拖曳到右邊的訓練區域（只有今天完成後不可再更動）", lagDuration);
    }

    private void CallCompleteAndHide()
    {
        CompleteAndHide("接著關閉訓練介面，讓角色開始訓練！");
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
    }

    //-----------------------------------------------------------------//

    public void Show(string contentText, float delay = 0f)
    {
        // 如果前一個動畫還在跑，先強行停止
        if (activeAnimation != null) StopCoroutine(activeAnimation);

        // 開始跑包含延遲的完整顯示序列
        if (!isOnceShown)
        {
            isOnceShown = true;
            activeAnimation = StartCoroutine(ShowSequence(contentText, delay));
        }
        else
        {
            // 已經出現過一次了，直接顯示內容但不跑動態
            tutorialBanner.SetActive(true);
            tutorialText.text = contentText;

            // 確保位置在可見狀態
            Vector2 pos = bannerRect.anchoredPosition;
            pos.y = visibleY;
            bannerRect.anchoredPosition = pos;
        }
    }

    private IEnumerator ShowSequence(string contentText, float delay) // 處理「先等待再動態」的流程
    {
        // 1. 如果有設定延遲時間就先等待
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // 2. 時間到了才正式啟用物件
        tutorialBanner.SetActive(true);
        tutorialText.text = contentText;

        // 3. 完整播放向上浮的動態
        yield return AnimateBanner(hiddenY, visibleY, true);
    }

    public void CompleteAndHide(string successText) // 動作完成後停留一下，再開始「緩慢向下滑動消除」
    {
        if (activeAnimation != null) StopCoroutine(activeAnimation);

        // 開始跑成功停留與下滑的序列
        activeAnimation = StartCoroutine(HideSequence(successText));
    }

    private IEnumerator HideSequence(string successText) // 停留與下滑協程
    {
        tutorialText.text = successText;
        // tutorialText.color = Color.green;

        // 1. 在畫面上硬性停留一段時間，讓玩家看清楚成功提示
        yield return new WaitForSeconds(stayDuration);

        // 2. 沿著原路徑滑下去
        yield return AnimateBanner(visibleY, hiddenY, false);

        // 3. 徹底關閉物件
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
