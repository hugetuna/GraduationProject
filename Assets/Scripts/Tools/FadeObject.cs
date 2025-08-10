using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//設置透明，掛靠此腳本的物件為可透明物件
public class FadeObject : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private Renderer renderer;
    //private Color originalColor;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    public void FadeOut()
    {
        renderer.enabled = false; // 隱藏
    }

    public void FadeIn()
    {
        renderer.enabled = true;  // 顯示
    }

    //private System.Collections.IEnumerator FadeTo(float targetAlpha)
    //{
    //    float startAlpha = mat.color.a;
    //    float t = 0f;
    //    while (t < fadeDuration)
    //    {
    //        t += Time.deltaTime;
    //        Color c = mat.color;
    //        c.a = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
    //        mat.color = c;
    //        yield return null;
    //    }
    //}
}
