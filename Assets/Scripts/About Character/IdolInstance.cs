using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdolInstance : MonoBehaviour
{
    //角色的基礎數值
    public Idols basicStatus;
    //實際運行的數值
    public int vocal;//歌唱力
    public int dance;//舞蹈力
    public int visual;//表現力
    public int charm;//魅力
    public int performance;//演技
    public int vigour;//體力
    public int vigourMax;//體力最大值
    //粉絲數
    public int fans;
    public int bondWithP;//與玩家的羈絆
    
    // Start is called before the first frame update
    void Start()
    {
        IdolSetUp();
    }
    
    //初始化
    private void IdolSetUp()
    {
        if (basicStatus == null)
        {
            Debug.LogError("基本狀態 (basicStatus) 未設定！");
            return;
        }
        vocal = basicStatus.vocal;
        dance = basicStatus.dance;
        visual = basicStatus.visual;
        charm = basicStatus.charm;
        performance = basicStatus.performance;
        vigour = vigourMax = basicStatus.vigour;
        fans = 0;
    }
    
    

}
