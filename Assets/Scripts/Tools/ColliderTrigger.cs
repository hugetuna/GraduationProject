using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    public UnityEvent Event;
    //傳送用
    public bool isTP;
    public string targetSceneName;
    public DialogueSaveData dialogueToTrigger;
    public StageAttribute stageToTrigger;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("傳送!");
        if (other.CompareTag("Player"))
        {
            Event.Invoke();
            if (isTP)
            {
                if (dialogueToTrigger != null) GameManager.Instance.SaveInkJSONAssetData(dialogueToTrigger);
                if (stageToTrigger != null) GameManager.Instance.SaveStageAttribute(stageToTrigger);
                SceneTransitionManager.Instance.teleportByTargetSceneName(targetSceneName);
            }
        }
    }

}
