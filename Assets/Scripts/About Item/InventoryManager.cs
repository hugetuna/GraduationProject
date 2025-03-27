using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
        Debug.Log("獲得道具：" + newItem.itemName);
    }

    public void UseItem(Item item)
    {
        item.Use();
        if (item.maxStack == 1) items.Remove(item);
    }
}
