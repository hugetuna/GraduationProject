using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmButtonHelper : MonoBehaviour, IPointerEnterHandler
{
    private Button _button;

    void Awake() => _button = GetComponent<Button>();

    // 當滑鼠移入按鈕時
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 強制將 EventSystem 的選取對象改為此按鈕
        // 這樣按下鍵盤空白鍵時，觸發的就是目前的按鈕
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
