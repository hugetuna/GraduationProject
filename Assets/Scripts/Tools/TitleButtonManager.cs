using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleButtonManager : MonoBehaviour
{
    public Canvas titleCanvas;
    public DialogueSaveData dialogueSaveDataNewgame;
    public Button StartButton;
    public Button LoadButton;
    public Button ConfigButton;
    public Button ExitButton;
    public AudioClip MainTitleBtnSnd;
    public AudioClip SecTitleBtnSnd;
    public AudioClip TitleSong;
    public void Start()
    {
        AudioManager.Instance.SetMusic(TitleSong);
        StartButton.onClick.AddListener(OnStartButtonClicked);
        LoadButton.onClick.AddListener(OnLoadButtonClicked);
        ConfigButton.onClick.AddListener(OnConfigButtonClicked);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }
    private void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySFX(MainTitleBtnSnd, setPitch: 1.2f);
        GameManager.Instance.SaveInkJSONAssetData(dialogueSaveDataNewgame);
        GameManager.Instance.ResetGame();
        DayManager.Instance.OnGameFileLoad();
        SceneTransitionManager.Instance.teleportByTargetSceneName("Dialogue Scene");
    }
    private void OnLoadButtonClicked()
    {
        AudioManager.Instance.PlaySFX(MainTitleBtnSnd, setPitch: 1.2f);
        GameManager.Instance.LoadFromFile();
    }
    private void OnConfigButtonClicked()
    {
        AudioManager.Instance.PlaySFX(SecTitleBtnSnd);
        GameManager.Instance.Canvas_Config.SetActive(true);
        //titleCanvas.gameObject.SetActive(false);
    }
    public void OnExitButtonClicked()
    {
        // AudioManager.Instance.PlaySFX(SecTitleBtnSnd, setPitch: 0.8f); // 在發出音效前就會退出了
        GameManager.Instance.QuitGame();
    }
}
