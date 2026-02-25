using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffBar : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private TextMeshProUGUI text; // 效果名稱物件
    [SerializeField] private Image icon; // 效果圖示物件

    public void UpdateBuffBar(string effectName, Sprite effectIcon = null)
    {
        text.text = effectName;
        if(effectIcon != null) icon.sprite = effectIcon;
    }
}
