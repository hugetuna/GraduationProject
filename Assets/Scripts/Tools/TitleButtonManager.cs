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
    public void Start()
    {
        StartButton.onClick.AddListener(OnStartButtonClicked);
        LoadButton.onClick.AddListener(OnLoadButtonClicked);
        ConfigButton.onClick.AddListener(OnConfigButtonClicked);
        ExitButton.onClick.AddListener(OnExitButtonClicked);
    }
    private void OnStartButtonClicked()
    {
        GameManager.Instance.SaveInkJSONAssetData(dialogueSaveDataNewgame);
        SceneTransitionManager.Instance.teleportByTargetSceneName("Dialogue Scene");
    }
    private void OnLoadButtonClicked()
    {
        GameManager.Instance.LoadFromFile();
    }
    private void OnConfigButtonClicked()
    {
        GameManager.Instance.Canvas_Config.SetActive(true);
        titleCanvas.gameObject.SetActive(false);
    }
    public void OnExitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
