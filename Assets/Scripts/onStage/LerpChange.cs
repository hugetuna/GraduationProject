using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//本腳本用作數字的插值上升
public class LerpChange : MonoBehaviour
{
    public TextMeshProUGUI scoreText;  // UI 元件
    public float lerpSpeed = 5f;       // 插值速度（數字越大越快）

    private int newText;           // 目標數(同時也是)
    private float beforeText;        // 畫面上顯示的數（用 float 來插值）
    // Start is called before the first frame update
    void Start()
    {
        newText = 0;
        beforeText = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(newText - beforeText) > 0.01f)
        {
            beforeText = Mathf.Lerp(beforeText, newText, Time.deltaTime * lerpSpeed);
            UpdateScoreText();
        }
    }
    //由外部使用以設置目標分
    public void SetText(int targetText)
    {
        newText = targetText;
    }
    //由內部更新數字並反映在UI
    private void UpdateScoreText()
    {
        scoreText.text = Mathf.RoundToInt(beforeText).ToString()+"pt";
    }
}
