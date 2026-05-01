using UnityEngine;
using UnityEngine.UI;

/* 掛在 GameManager 底下的 ConfigCanvas 上 */
public class ConfigCanvasSFX : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button backButtonX;
    [SerializeField] private AudioClip backSound;

    void Start()
    {
        backButton.onClick.AddListener(OnBackButtonsClicked);
        backButtonX.onClick.AddListener(OnBackButtonsClicked);
    }

    private void OnBackButtonsClicked()
    {
        // 關閉功能已在 Inspector 設定
        if (backSound != null)
        {
            AudioManager.Instance.PlaySFX(backSound);
        }
    }
}
