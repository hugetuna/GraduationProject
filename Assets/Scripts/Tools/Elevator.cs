using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    public string InteractionKey => null; // 這個字串用來指定動畫 key
    public GameObject elevatorCanvas;
    public SetElevatorIcon elevatorIconSetter;
    public int placeCode; // 電梯所在樓層代碼
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Interact(int tool) {
        elevatorCanvas.SetActive(true);
        elevatorIconSetter.SetElevatorIconPos(placeCode);
    }

}
