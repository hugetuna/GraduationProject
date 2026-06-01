using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ClothChange : MonoBehaviour,IInteractable
{
    public string InteractionKey => "ClothChange"; // 這個字串用來指定動畫 key
    public TeamManager teamManager;
    public IdolInClothChangeManager idolInClothChangeManager;
    public Dictionary<string, SpriteResolver> resolvers = new Dictionary<string, SpriteResolver>();
    public Canvas canvas_ClothChange;
    public void Interact(int tool)
    {
        canvas_ClothChange.gameObject.SetActive(true);
        idolInClothChangeManager.SetupIdolWhoInClothChange();
        UIAndPlayerInput.DisableAllPlayerInputs();
    }
}
