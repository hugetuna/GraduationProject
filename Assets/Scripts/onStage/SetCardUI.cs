using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;          // 控制 Image、Button、Slider 等 UI 元件
using TMPro;                  // 控制 TextMeshPro 的文字內容

public class SetCardUI : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI pointText;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void SetCard(ActionCard cardToSet)
    {
        pointText.text = cardToSet.point.ToString();
    }
}
