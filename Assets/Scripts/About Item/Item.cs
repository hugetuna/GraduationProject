using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Consumable, Equipment}

public abstract class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public int maxStack = 1;

    public abstract void Use();
}
