using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BlurRenderer : MonoBehaviour
{
    [Header("指定渲染背景的相機 (拍背景 + 角色立繪)")]
    public Camera blurCamera;
    [Header("模糊材質 (Shader Graph or Custom Shader)")]
    public Material blurMaterial;

    private RenderTexture blurRT;

    

    [ContextMenu("UpdateTexture")]
    public void UpdateTexture()
    {
        if (blurCamera == null || blurMaterial == null)
        {
            Debug.LogWarning("BlurRenderer 缺少相機或材質！");
            return;
        }
        blurRT = new RenderTexture(Screen.width, Screen.height, 16);
        blurRT.name = "BlurRenderTexture";
        blurRT.Create();

        // 指定相機輸出到 RT
        blurCamera.targetTexture = blurRT;
        // 指定材質使用的貼圖
        blurMaterial.SetTexture("_RenTex", blurRT);

        Debug.Log("模糊 RenderTexture 更新成功");
    }

    
}
