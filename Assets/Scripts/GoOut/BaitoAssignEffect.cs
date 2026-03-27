using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在角色的 UI 圖片上 */
public class BaitoAssignEffect : MonoBehaviour
{
    private Image image; // 角色圖片的 Image 元件參考
    private DragToBaito drag; // 拖曳元件參考
    [SerializeField] private TextMeshProUGUI statHintText; // 顯示角色狀態的文字（是否已出發打工）

    void Awake()
    {
        image = GetComponent<Image>();
        drag = GetComponent<DragToBaito>();
    }

    void Start()
    {
        statHintText.gameObject.SetActive(false);
    }

    public void Initialize(Baito idolBaito)
    {
        if(idolBaito == null) // 角色還沒有去任何打工
        {
            statHintText.gameObject.SetActive(false);
            image.color = new Color(1, 1, 1, 1f);
            // 預設啟用拖曳（若體力不足再禁用）
        }
        else
        {
            UpdateCharacterStatus(idolBaito, true); // 角色已經去打工了，同步狀態顯示
        }
    }

    public void UpdateCharacterStatus(Baito baitoData, bool justAssigned)
    {
        // 如果角色在打工區且尚未出發，則更新狀態顯示
        if (drag.CurrentDropZone.zoneType == BaitoDropZoneType.Baito && justAssigned)
        {
            statHintText.text = baitoData.baitoName; // 更新狀態提示文字
            statHintText.gameObject.SetActive(true);
            image.color = new Color(1, 1, 1, 0.5f); // 降低圖片透明度
            drag.enabled = false; // 禁用拖曳功能
        }
    }
}
