using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmButtonHelper : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;
    public TMPro.TextMeshProUGUI ButtonText;
    public TMPro.TextMeshProUGUI countingText;

    void Awake() => _button = GetComponent<Button>();

    // 當滑鼠移入按鈕時
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 強制將 EventSystem 的選取對象改為此按鈕
        // 這樣按下鍵盤空白鍵時，觸發的就是目前的按鈕
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
    public void OnSelected()
    {
        ButtonText.color=new Color32(249, 180, 195, 255);
        if (countingText != null) countingText.color = new Color32(249, 180, 195, 255);
    }
    public void OnDeselected()
    {
        ButtonText.color = Color.white;
        if (countingText != null) countingText.color = Color.white;
    }
}
