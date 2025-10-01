using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[System.Serializable]
[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueSaveData:ScriptableObject
{
    public TextAsset inkJSONAsset;
    public string backToSceneName;
}
