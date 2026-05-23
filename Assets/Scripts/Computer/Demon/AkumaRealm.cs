using UnityEngine;

/* 掛在 AkumaRealm 物件上，讓惡魔桌寵可以用來判斷自身位置是否超出範圍 */
public class AkumaRealm : MonoBehaviour
{
    [Header("範圍設定 (相對於此物件中心)")]
    [SerializeField] private float width = 5f;
    [SerializeField] private float height = 3f;

    public Vector3 ClampToMoveArea(Vector3 targetPos) // 取得桌寵投射在平面範圍上的座標
    {
        // 轉為局部座標計算，這樣即便物件傾斜也能正確限制
        Vector3 localPos = transform.InverseTransformPoint(targetPos);

        float halfW = width / 2f;
        float halfH = height / 2f;

        localPos.x = Mathf.Clamp(localPos.x, -halfW, halfW);
        localPos.y = Mathf.Clamp(localPos.y, -halfH, halfH);
        localPos.z = 0; // 確保它永遠貼在平面上

        return transform.TransformPoint(localPos);
    }

    private void OnDrawGizmos() // 在 Scene 視窗畫出綠色框框，方便調整範圍
    {
        Gizmos.color = Color.green;
        // 設置 Gizmos 的矩陣，使其跟隨物件的旋轉與縮放
        Gizmos.matrix = transform.localToWorldMatrix;
        
        // 畫出一個線框立方體 (z 設很薄就像一個面)
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(width, height, 0.01f));
        
        // 畫一個半透明的面方便辨識
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(width, height, 0.01f));
    }
}