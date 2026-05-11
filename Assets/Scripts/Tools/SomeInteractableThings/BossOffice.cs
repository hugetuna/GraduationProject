using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class BossOffice : MonoBehaviour, IInteractable
{
    public string InteractionKey => "BossOffice"; // 這個字串用來指定動畫 key
    public void Interact(int tool)
    {
        
    }
}
