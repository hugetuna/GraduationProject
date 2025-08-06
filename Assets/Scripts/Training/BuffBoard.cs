using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffBoard : MonoBehaviour
{
    private TrainingUIData trainingUIData; // 取用當前訓練 UI 資料
    public VigourBar vigourBar; // 取用 VigourBar 的參考
    private IdolInstance characterInfo; // 該角色的數值資料（從 VigourBar 取得）
    public Text buffText; // 顯示訓練加乘效果的文字

    void Start()
    {
        buffText.text = "";
        trainingUIData = vigourBar.trainingUIData;
    }

    
    void Update()
    {
        characterInfo = vigourBar.characterInfo;
        buffText.text = "DANCE收益+10%" + "\n" + "魅力+"; // 尚未實作 buff 功能，暫時先寫死文字
    }
}
