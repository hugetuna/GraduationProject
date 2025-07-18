using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//以連續圖片呈現2D動畫
public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public List<Sprite> frames; // 動畫的所有幀
    public float frameRate = 12f; // 每秒多少幀

    private int currentFrame;
    private float timer;

    void Update()
    {
        if (frames == null || frames.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }

    // 你可以在需要時切換動畫
    public void SetFrames(List<Sprite> newFrames)
    {
        frames = newFrames;
        currentFrame = 0;
        timer = 0;
    }

}
