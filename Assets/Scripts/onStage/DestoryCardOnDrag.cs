using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DestoryCardOnDrag : MonoBehaviour, IDropHandler
{
    private OnStageManager stageManager;
    public AudioClip dropCardSFX;
    // Start is called before the first frame update
    void Start()
    {
        stageManager = FindAnyObjectByType<OnStageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDrop(PointerEventData eventData)
    {
        // 嘗試從拖曳來源取得 SetCardUI
        SetCardUI draggedCardUI = eventData.pointerDrag?.GetComponent<SetCardUI>();
        if (draggedCardUI != null)
        {
            ActionCard incomingCard = draggedCardUI.cardData;
            bool accepted = incomingCard!=null;
            if (accepted&& draggedCardUI.isCard)
            {
                Debug.Log($"丟棄卡片 {incomingCard.cardName}！");
                stageManager.hands.Remove(draggedCardUI.gameObject);
                stageManager.Grave.Add(incomingCard);
                Destroy(draggedCardUI.gameObject); // 卡片被使用後消失
                AudioManager.Instance.PlaySFX(dropCardSFX);
            }
            else
            {
                Debug.Log($"無法丟棄該卡片。");
            }
        }
    }
}
