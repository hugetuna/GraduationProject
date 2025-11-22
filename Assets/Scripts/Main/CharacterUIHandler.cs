using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject CharacterUI;
    [SerializeField] private Button CharacterButton;
    [SerializeField] private Button panelBackground;
    //-----------------------------------------------------------------//
    [SerializeField] private AudioClip openCharacterSound;
    
    void Start()
    {
        CharacterUI.SetActive(false);
        CharacterButton.onClick.AddListener(OpenCharacterUI); // 設置按鈕點擊事件
        panelBackground.onClick.AddListener(() =>
        {
            if (CharacterUI.activeSelf)
            {
                UIAndPlayerInput.EnableAllPlayerInputs(); // 啟用所有玩家的輸入系統
                CharacterUI.SetActive(false);
            }
        });
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
}
