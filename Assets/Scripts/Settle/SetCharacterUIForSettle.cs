using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在結算畫面的各個角色根部 */
public class SetCharacterUIForSettle : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Image headImage; // 從 SetSettleUI 傳進來的頭像
    [SerializeField] private TextMeshProUGUI nameTextEffect; // 文字特效
    [SerializeField] private TextMeshProUGUI nameText;
    //-----------------------------------------------------------------//
    [SerializeField] private Image vigourBar; // 會變化的圖片部分
    [SerializeField] private TextMeshProUGUI vigourMaxDeltaText;
    //-----------------------------------------------------------------//
    [SerializeField] private TextMeshProUGUI danceText;
    [SerializeField] private TextMeshProUGUI danceDeltaText;
    [SerializeField] private TextMeshProUGUI vocalText;
    [SerializeField] private TextMeshProUGUI vocalDeltaText;
    [SerializeField] private TextMeshProUGUI visualText;
    [SerializeField] private TextMeshProUGUI visualDeltaText;
    //-----------------------------------------------------------------//
    [SerializeField] private TextMeshProUGUI performanceText;
    [SerializeField] private TextMeshProUGUI performanceDeltaText;
    //-----------------------------------------------------------------//
    // [Header("顯示資料")]

    public void ShowCharacterBenefits(Sprite headSprite, string characterName,
        int vigourCurrent, int vigourMax, int vigourMaxDelta,
        int danceCurrent, int danceDelta,
        int vocalCurrent, int vocalDelta,
        int visualCurrent, int visualDelta,
        int performanceCurrent, int performanceDelta)
    {
        // 頭像和名字
        headImage.sprite = headSprite;
        nameTextEffect.text = characterName;
        nameText.text = characterName;

        // 體力
        vigourBar.fillAmount = (float)vigourCurrent / vigourMax;
        vigourMaxDeltaText.text = (vigourMaxDelta > 0) ? $"+{vigourMaxDelta}" : "";

        // 三種能力數值
        danceText.text = danceCurrent.ToString();
        danceDeltaText.text = (danceDelta > 0) ? $"+{danceDelta}" : "";

        vocalText.text = vocalCurrent.ToString();
        vocalDeltaText.text = (vocalDelta > 0) ? $"+{vocalDelta}" : "";

        visualText.text = visualCurrent.ToString();
        visualDeltaText.text = (visualDelta > 0) ? $"+{visualDelta}" : "";

        // 演技
        performanceText.text = performanceCurrent.ToString();
        performanceDeltaText.text = (performanceDelta > 0) ? $"+{performanceDelta}" : "";
    }

}
