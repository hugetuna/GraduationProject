using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBlurManager : MonoBehaviour
{
    [Header("UI 設定")]
    public RawImage blurDisplay; // 拖入對話框背景的 RawImage
    public GameObject dialogContent; // 你的對話內容（截圖時需先隱藏，避免把自己拍進去）

    [Header("模糊參數")]
    public Material blurMaterial; // 使用上面的 SimpleBlur Shader 建立的材質
    [Range(1, 4)] public int downsample = 2; // 降採樣：數值越高越模糊且效能越好
    [Range(1, 8)] public int iterations = 3; // 迭代次數：模糊幾次

    // 當物件被啟用（對話框跳出來）時觸發
    void OnEnable()
    {
        if (blurDisplay != null) StartCoroutine(CaptureAndBlur());
    }

    IEnumerator CaptureAndBlur()
    {
        // 1. 隱藏對話框內容，避免截圖截到對話框本身
        if (dialogContent != null) dialogContent.SetActive(false);
        blurDisplay.enabled = false;

        // 2. 等待當前幀渲染結束（確保背景都畫好了）
        yield return new WaitForEndOfFrame();

        // 3. 建立一個較小的 RenderTexture (降採樣)
        int width = Screen.width / downsample;
        int height = Screen.height / downsample;
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0);

        // 4. 截取螢幕畫面到 rt
        ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);

        // 5. 進行多次模糊迭代
        for (int i = 0; i < iterations; i++)
        {
            RenderTexture temp = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(rt, temp, blurMaterial);
            RenderTexture.ReleaseTemporary(rt);
            rt = temp;
        }

        // 6. 將結果交給 UI 顯示
        blurDisplay.texture = rt;
        blurDisplay.enabled = true;

        // 7. 恢復對話框內容
        if (dialogContent != null) dialogContent.SetActive(true);
    }

    void OnDisable()
    {
        // 關閉時釋放記憶體，避免洩漏
        if (blurDisplay.texture is RenderTexture rt)
        {
            RenderTexture.ReleaseTemporary(rt);
            blurDisplay.texture = null;
        }
    }
}
