using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour, IInteractable
{
    public GameObject elevatorSelection;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Interact(int tool) {
        elevatorSelection.SetActive(true);
    }

}
