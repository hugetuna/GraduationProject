using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class TachieSlot
{
    public string slotName;
    public Image image;
    public string currentCharacter;
    public string currentEmotion;
}
public class TachieManager : MonoBehaviour
{
    public List<TachieSlot> tachieSlots;

}
