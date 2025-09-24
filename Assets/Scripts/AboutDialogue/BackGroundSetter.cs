using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
[System.Serializable]
public class BackGroundFile
{
    public string BackGroundName;
    public Sprite BackGroundImg;
}
public class BackGroundSetter : MonoBehaviour
{
    public Image BackGroundImgNow;
    public List<BackGroundFile> backGroundFiles;
    public void SetBackGround(string TargetBackGroundTag)
    {
        BackGroundFile hitFile = backGroundFiles.Find(file => file.BackGroundName == TargetBackGroundTag);
        BackGroundImgNow.sprite = hitFile.BackGroundImg;
    }
}
