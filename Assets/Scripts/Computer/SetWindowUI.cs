using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* 掛在視窗本身，預計透過特定方式來設定視窗種類 */
public class SetWindowUI : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
        // gameObject.SetActive(false); // 初始時隱藏視窗 -> 改成手動預設
    }

    // void Update()
    // {

    // }

    public void OnPointerClick(PointerEventData eventData) 
    {
        // 點擊視窗以將其置頂（於父元件中）
        transform.SetAsLastSibling();
    }
}
