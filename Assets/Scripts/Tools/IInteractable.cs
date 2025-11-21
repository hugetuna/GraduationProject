using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(int tool); // 互動行為
    string InteractionKey { get; } // 用來指定互動字串（只讀屬性）
}
