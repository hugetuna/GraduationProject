using System.IO;
using UnityEngine;

public class TransparentExporter : MonoBehaviour
{
    public Camera targetCamera;
    public int imageWidth = 1024;  // 輸出的圖片寬度
    public int imageHeight = 1024; // 輸出的圖片高度

    public void CaptureTransparentPNG(string fileName)
    {
        // 建立一個支援 Alpha (透明通道) 的 RenderTexture
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32);
        targetCamera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        targetCamera.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        screenShot.Apply();

        // 釋放記憶體
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 編碼為 PNG 並存檔
        byte[] bytes = screenShot.EncodeToPNG();
        // 圖片會存放在你 Unity 專案的 Assets 資料夾下
        string path = Path.Combine(Application.dataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);
        
        Destroy(screenShot);
    }
}