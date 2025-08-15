using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在視窗本身，以控制其最小化與還原 */
public class MinimizeAnimation : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [Tooltip("最小化與放大動畫的時間")]
    [SerializeField]private float duration = 0.4f;  // 動畫時間

    [Tooltip("視窗的彈性效果")]
    [SerializeField] private float overshoot = 1.1f; // 彈出比例 (比原本大10%)

    private Vector3 originalScale; // 原先比例

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }

    public void Minimize() // 最小化視窗
    {
        StopAllCoroutines();
        StartCoroutine(AnimateWindow(originalScale, Vector3.zero, 1f, 0f, false, true));
    }

    public void Restore()  // 開啟最小化視窗
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;

        StopAllCoroutines();
        StartCoroutine(AnimateWindow(Vector3.zero, originalScale, 0f, 1f, true, false));
    }

    private IEnumerator AnimateWindow(Vector3 startScale, Vector3 endScale, float startAlpha, float endAlpha, bool enableAtEnd, bool reverseBounce)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            // 彈性公式（OutBack 或 InBack）
            float easedT;
            if (!reverseBounce) // 還原時彈出
                easedT = EaseOutBack(t);
            else                // 最小化時彈入
                easedT = EaseInBack(t);

            transform.localScale = Vector3.LerpUnclamped(startScale, endScale, easedT);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        transform.localScale = endScale;
        canvasGroup.alpha = endAlpha;

        if (!enableAtEnd)
            gameObject.SetActive(false);
    }

    // EaseOutBack（彈出感）
    private float EaseOutBack(float t)
    {
        float s = overshoot; 
        t -= 1;
        return t * t * ((s + 1) * t + s) + 1;
    }

    // EaseInBack（彈入感）
    private float EaseInBack(float t)
    {
        float s = overshoot; 
        return t * t * ((s + 1) * t - s);
    }
}
