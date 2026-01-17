using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    public void TriggerEnd()
    {
        GameManager.Instance.QuitGame();
    }
}
