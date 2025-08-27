using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewIdol", menuName = "Idol")]

public class Idols : ScriptableObject
{
    //舞台圖片組
    public List<Sprite> idleFrames;
    public List<Sprite> actionFrames;
    //其他基本數值
    public string idolName;  // 角色名稱
    public int vocal;//歌唱力
    public int dance;//舞蹈力
    public int visual;//表現力
    public int charm;//魅力
    public int performance;//演技
    public int vigour;//體力
    //舞台基本數值
    public int onStageStamina;//舞台體力
}
