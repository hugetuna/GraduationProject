using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* 掛在提醒玩家已使用某道具的提示 prefab 上 */
public class PackHintToggler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hintText; // 提示文字
    [SerializeField] private TextMeshProUGUI descriptionText; // 道具效果說明文字
    [SerializeField] private Button okButton; // 確認按鈕
    [SerializeField] private AudioClip closeSound;

    public void SetHintUI(string idolName, Item item)
    {
        string itemName = item.itemName;
        string itemDescription = item.description;
        string hint = "";
        
        if(itemName.Length > 5) hint = $"{idolName}使用了\n{itemName}！";
        else hint = $"{idolName}使用了{itemName}！";
        hintText.text = hint;
        descriptionText.text = $"道具效果：{itemDescription}";
        okButton.onClick.AddListener(OnOkButtonClicked);
    }

    private void OnOkButtonClicked()
    {
        AudioManager.Instance.PlaySFX(closeSound); // 播放音效
        Destroy(gameObject, 0.1f); // 延遲 0.1 秒後銷毀提示物件
    }
}
