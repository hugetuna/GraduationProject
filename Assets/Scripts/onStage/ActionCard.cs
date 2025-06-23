using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ActionCard", menuName = "Stage/Action Card")]
public class ActionCard : ScriptableObject
{
    public string cardId;
    public string cardName;
    public Sprite cardPic;
    //分數限制
    public int voGate;
    public int daGate;
    public int viGate;
    //加分量和效果
    public int point;
    public string effectString;
}
