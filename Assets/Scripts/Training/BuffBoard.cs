using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffBoard : MonoBehaviour
{
    // private IdolInstance characterInfo; // 該角色的數值資料（從 VigourBar 取得）
    [SerializeField] private TextMeshProUGUI buffText; // 顯示訓練加乘效果的文字

    void Start()
    {
        buffText.text = "";
    }
    void OnEnable()
    {
        DragToLesson.OnEnableOrEndDrag += UpdateBuffBoard; // 訂閱拖曳結束事件    
    }

    void OnDisable()
    {
        DragToLesson.OnEnableOrEndDrag -= UpdateBuffBoard; // 取消訂閱拖曳結束事件    
    }

    public void UpdateBuffBoard(TrainingUIData trainingUIData)
    {
        // characterInfo = vigourBar.CharacterInfo;

        buffText.text = "DANCE收益+10%" + "\n" + "魅力+"; // 尚未實作 buff 功能，暫時先寫死文字
    }
}
