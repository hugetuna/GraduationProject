using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* 掛在 UIManager 上 */
public class CharacterUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject CharacterUI;
    [SerializeField] private Button CharacterButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button panelBackground;
    //-----------------------------------------------------------------//
    [SerializeField] private AudioClip openCharacterSound;
    [SerializeField] private AudioClip closeCharacterSound;
    
    void Start()
    {
        CharacterUI.SetActive(false);
        CharacterButton.onClick.AddListener(OpenCharacterUI); // 設置按鈕點擊事件
        closeButton.onClick.AddListener(CloseCharacterUI);
        panelBackground.onClick.AddListener(CloseCharacterUI);
    }

    private void OpenCharacterUI()
    {
        if (!CharacterUI.activeSelf) // 如果角色 UI 未開啟，則打開它
        {
            UIAndPlayerInput.DisableAllPlayerInputs(); // 禁用所有玩家的輸入系統
            AudioManager.Instance.PlaySFX(openCharacterSound); // 播放音效
            CharacterUI.SetActive(true);
        }
    }

    private void CloseCharacterUI() // 使用 UI 上的叉叉關閉 UI
    {
        Debug.Log("關閉 UI");
        UIAndPlayerInput.EnableAllPlayerInputs(); // 啟用所有玩家的輸入系統
        AudioManager.Instance.PlaySFX(closeCharacterSound); // 播放音效
        CharacterUI.SetActive(false);
    }
}
