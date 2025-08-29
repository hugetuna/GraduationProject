using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
    public GameObject panelToClose;
    private Button closeButton;

    void Awake()
    {
        closeButton = GetComponent<Button>();   
    }

    void Start()
    {
        closeButton.onClick.AddListener(CloseThisPanel);
    }

    public void CloseThisPanel()
    {
        // 可使用 UI 上的叉叉關閉 UI
        Debug.Log("關閉 UI");
        panelToClose.SetActive(false);
    }
}
