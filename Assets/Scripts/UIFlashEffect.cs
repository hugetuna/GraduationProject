using System.Collections;
using UnityEngine;

public class UIFlashEffect : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // 建議在物件上掛個 CanvasGroup，控制透明度最省事
        if (TryGetComponent<CanvasGroup>(out var cg))
        {
            canvasGroup = cg;
        }
        else
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Flash(int times = 3, float speed = 0.1f)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(times, speed));
    }

    private IEnumerator FlashRoutine(int times, float speed)
    {
        for (int i = 0; i < times; i++)
        {
            canvasGroup.alpha = 0.2f; // 變淡
            yield return new WaitForSeconds(speed);
            canvasGroup.alpha = 1.0f; // 變亮
            yield return new WaitForSeconds(speed);
        }
    }
}
