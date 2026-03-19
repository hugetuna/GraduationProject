using UnityEngine;
using UnityEngine.UI;

/* 掛在商演介面上 */
public class SetActivityUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Button closeButton; // 關閉介面的按鈕
    
    void Start()
    {
        closeButton.onClick.AddListener(CloseActivityUI); // 為關閉按鈕添加點擊事件
    }

    private void CloseActivityUI()
    {
        gameObject.SetActive(false);
    }
}
