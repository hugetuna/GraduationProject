using UnityEngine;
using System.IO;

public class UISnapshotTool : MonoBehaviour
{
    public Camera captureCamera;
    public RectTransform targetUI; // 你想拍的 UI 物件
    public string fileName = "UIIcon";

    [ContextMenu("執行截圖 (存為 PNG)")]
    public void TakeSnapshot()
    {
        if (captureCamera == null || targetUI == null) return;

        // 1. 根據 UI 的大小設定圖片尺寸
        int width = (int)targetUI.rect.width;
        int height = (int)targetUI.rect.height;

        // 2. 建立 RenderTexture
        RenderTexture rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
        captureCamera.targetTexture = rt;

        // 3. 渲染相機畫面
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        captureCamera.Render();

        // 4. 讀取像素
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        // 5. 存檔到 Project 資料夾
        byte[] bytes = texture.EncodeToPNG();
        string path = Application.dataPath + "/" + fileName + ".png";
        File.WriteAllBytes(path, bytes);

        // 6. 清理
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        Debug.Log("UI 已成功存檔至: " + path);

        // 自動刷新 Project 視窗看到新圖片
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
