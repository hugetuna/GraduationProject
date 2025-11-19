using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffBoard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buffText; // 顯示訓練加乘效果的文字
    
    // private IdolInstance characterInfo; // 該角色的數值資料
    // private string characterName; // 該角色名稱
    // private DragToLesson dragToLesson; // 取得該角色的 DragToLesson 參考（判斷當前拖曳區域）
    
    // void Awake()
    // {
    //     dragToLesson = GetComponent<DragToLesson>();
    // }

    public void Initialize(string myName)
    {
        buffText.text = "DANCE收益+10%" + "\n" + "魅力+"; // 尚未實作 buff 功能，暫時先寫死文字
    }
    
    public void UpdateBuffBoard(TrainingUIData trainingUIData)
    {
        buffText.text = "DANCE收益+10%" + "\n" + "魅力+"; // 尚未實作 buff 功能，暫時先寫死文字
    }
}
