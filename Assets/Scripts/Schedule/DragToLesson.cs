using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragToLesson : MonoBehaviour
{

    Vector3 mousePosition;

    private Vector3 GetMousePos(){
        // 取得被拖曳物件的鏡頭座標
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown(){
        // 在滑鼠左鍵被點擊時取得滑鼠座標
        mousePosition = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag(){
        // 滑鼠拖曳時讓物件跟著滑鼠移動
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
}
