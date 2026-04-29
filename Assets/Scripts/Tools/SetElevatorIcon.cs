using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SetElevatorIcon : MonoBehaviour
{
    public GameObject elevatorIcon;
    public GameObject elevatorIconPeoples;
    public List<Button> buttons; // 按鈕列表
    public List<Transform> elevatorIconPositions; // 電梯圖示位置列表
    public List<Sprite> peopleSprites; // 電梯內人數對應的圖片列表
    [Header("音效設定")]
    public AudioClip selectFloorSound; // 選擇樓層的音效
    public AudioClip exitSound; // 退出電梯選單的音效
    public Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            if (exitSound != null) AudioManager.Instance.PlaySFX(exitSound);
        });
    }

    public void SetElevatorIconPos(int placeCode)
    {
        int i = 0;
        // 根據 placeCode 設定電梯圖示位置(0代表地下一樓，其餘為樓層)
        elevatorIcon.transform.position = elevatorIconPositions[placeCode].position;
        // 為每個按鈕添加對應的場景切換事件
        for (i = 0; i < buttons.Count; i++)
        {
            int index = i; // 捕獲當前的索引值
            UnityEvent unityEvent = new UnityEvent();
            if (DayManager.Instance.date == 1)//第一天禁止進入B1，封鎖按鈕
            {
                buttons[0].interactable = false;
            }
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                if (selectFloorSound != null) AudioManager.Instance.PlaySFX(selectFloorSound);
                if (SceneTransitionManager.Instance != null)
                    if (index == 0)
                    {
                        SceneTransitionManager.Instance.teleportByTargetSceneName("Floor_B1");
                    }
                    else
                        SceneTransitionManager.Instance.teleportByTargetSceneName("Floor_" + index);
                else
                    Debug.LogError("SceneTransitionManager.Instance 為 null！");
            });
        }
    }
    public void SetElevatorIconPeople(int peopleCount)
    {
        elevatorIconPeoples.GetComponent<UnityEngine.UI.Image>().sprite = peopleSprites[peopleCount];
    }
}
