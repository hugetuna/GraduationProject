using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在需要震動效果的 UI 元素上 */
public class UIShakeEffect : MonoBehaviour
{
    private Vector2 originalPosition;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void Shake(float duration = 0.3f, float magnitude = 15f)
    {
        // 停止正在進行的震動，避免重疊
        StopAllCoroutines(); 
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 在 X 軸上產生隨機偏移
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            
            // 隨著時間增加，震動幅度逐漸減小 (衰減)
            float damping = 1.0f - (elapsed / duration);
            rectTransform.anchoredPosition = originalPosition + new Vector2(xOffset * damping, 0);

            elapsed += Time.deltaTime;
            yield return null; // 等待下一幀
        }

        // 結束後歸位
        rectTransform.anchoredPosition = originalPosition;
    }
}
