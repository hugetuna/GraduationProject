using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class fitScreen : MonoBehaviour
{
    void Start()
    {
        FitToCamera();
    }

    void FitToCamera()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null || Camera.main == null) return;

        // 取得相機可視高度與寬度
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        // 取得 sprite 實際世界單位尺寸
        Vector2 spriteSize = sr.sprite.bounds.size;

        // 計算比例
        float scaleX = screenWidth / spriteSize.x;
        float scaleY = screenHeight / spriteSize.y;

        // 為避免變形，取較大比例確保畫面填滿
        float finalScale = Mathf.Max(scaleX, scaleY);
        transform.localScale = new Vector3(finalScale, finalScale, 1f);
    }
}
