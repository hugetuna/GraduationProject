using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在販賣頁面的各個角色根部 */
public class SetCharacterUIForSell : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Image headImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button lastButton; // 上一頁按鈕
    [SerializeField] private Button nextButton; // 下一頁按鈕
    [SerializeField] private TextMeshProUGUI pageText; // 頁數顯示
    
    // 所有放粉絲的位置＆相關程式碼

    void Start()
    {
        headImage.sprite = null;
        nameText.text = "";
        lastButton.interactable = false;
        nextButton.interactable = false;
        pageText.text = "1/1";
    }

    public void Initialize(IdolInstance idol)
    {
        headImage.sprite = idol.spriteTachie;
        nameText.text = idol.idolIndex.ToString();

        // 從 ResourceManager 取得該 Idol 的 FansItem 清單
        // 再根據粉絲數量決定頁數與按鈕顯示
    }
}
