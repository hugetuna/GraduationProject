using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* 掛在提醒玩家已使用某道具的提示 prefab 上 */
public class PackHintToggler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintText; // 提示文字 UI 元素
    [SerializeField] private Button okButton; // 確認按鈕

    public void SetHintUI(string idolName, string itemName)
    {
        string hint = $"{idolName} 使用了「{itemName}」！";
        hintText.text = hint;
        okButton.onClick.AddListener(OnOkButtonClicked);
    }

    private void OnOkButtonClicked()
    {
        Destroy(gameObject, 0.1f); // 延遲 0.1 秒後銷毀提示物件
    }
}
