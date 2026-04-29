using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    public string InteractionKey => "Elevator"; // 這個字串用來指定動畫 key
    public GameObject elevatorCanvas;
    public SetElevatorIcon elevatorIconSetter;
    public int placeCode; // 電梯所在樓層代碼
    public AudioClip showElevatorCanvasSFX; // 顯示電梯選單的音效
    public AudioClip elevatorDingSFX;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.isElevatorUsedToday == true)
        {
            AudioManager.Instance.PlaySFX(elevatorDingSFX);
        }
        GameManager.Instance.isElevatorUsedToday = true;
    }
    public void Interact(int tool) {
        if(showElevatorCanvasSFX != null) AudioManager.Instance.PlaySFX(showElevatorCanvasSFX);
        elevatorCanvas.SetActive(true);
        elevatorIconSetter.SetElevatorIconPos(placeCode);
    }

}
