using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    public string InteractionKey => null; // 這個字串用來指定動畫 key
    public GameObject elevatorCanvas;
    public SetElevatorIcon elevatorIconSetter;
    public int placeCode; // 電梯所在樓層代碼
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
        elevatorCanvas.SetActive(true);
        elevatorIconSetter.SetElevatorIconPos(placeCode);
    }

}
