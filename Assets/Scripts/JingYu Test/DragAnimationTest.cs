using UnityEngine;

public class DragCharacter : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Animator animator;
    private bool isFlipped = false;
    public Transform footPosition; // 設定角色的腳部位置

    void Start()
    {
        animator = GetComponent<Animator>(); // 取得 Animator
    }

    void OnMouseDown()
    {
        // 紀錄原始偏移
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
        SetDraggedState(true);

        // 翻轉角色
        if (!isFlipped)
        {
            FlipCharacter();
        }

        // 讓角色腳部對準鼠標
        transform.position = GetMouseWorldPos() - (footPosition.position - transform.position);
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;
        SetDraggedState(false);

        // 放開後恢復角色朝向
        if (isFlipped)
        {
            ResetCharacterRotation();
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void SetDraggedState(bool state)
    {
        isDragging = state;
        if (animator != null)
        {
            animator.SetBool("isDragged", state);
        }
    }

    private void FlipCharacter()
    {
        isFlipped = true;
        
        // 翻轉角色 180 度
        transform.Rotate(0f, 0f, 180f);

        // 讓腳部對準滑鼠位置
        transform.position = GetMouseWorldPos() - (footPosition.position - transform.position);
        
        // 重新計算 offset
        offset = transform.position - GetMouseWorldPos();
    }

    private void ResetCharacterRotation()
    {
        isFlipped = false;

        // 回到正向
        transform.Rotate(0f, 0f, -180f);
    }
}
