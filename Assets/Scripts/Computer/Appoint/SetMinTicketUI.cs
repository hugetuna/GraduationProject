using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/* 掛在最小化活動票券的 prefab 根部 */
public class SetMinTicketUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Activity activity;
    [SerializeField] private TextMeshProUGUI activityNameText; // 活動名稱文字
    [SerializeField] private GameObject hoverInfo; // 滑鼠懸停時顯示的資訊物件
    [SerializeField] private TextMeshProUGUI feeText; // 活動價格文字
    [SerializeField] private TextMeshProUGUI vigourCostText; // 活動耗體文字

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickMinTicket);
    }
    
    public void Initialize(Activity activity)
    {
        this.activity = activity;
        
        // 設定 UI 顯示
        activityNameText.text = activity.activityName;
        feeText.text = $"-{activity.fee}";
        vigourCostText.text = $"-{activity.vigourCost}";

        // 確保字型正確渲染
        activityNameText.ForceMeshUpdate();
        feeText.ForceMeshUpdate();
        vigourCostText.ForceMeshUpdate();
    }

    // 當滑鼠游標「進入」物件範圍時觸發
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter " + gameObject.name);
        hoverInfo.SetActive(true);
    }

    // 當滑鼠游標「離開」物件範圍時觸發
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverInfo.SetActive(false);
    }

    public void OnClickMinTicket() 
    {
        // 點擊最小化票券也能觀看活動資訊
        GetComponentInParent<TicketInfoUI>().UpdateTicketInfoUI(null, activity);
    }

    public Activity GetActivity()
    {
        return activity;
    }
}
