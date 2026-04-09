using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorriorFaceChange : MonoBehaviour
{
    [SerializeField]
    public Image Image;//需要更换表情的Image组件
    public List<Sprite> AllIdolSprites;//所有偶像的表情圖
    void Start()
    {
        ChangeFace();
    }
    public void ChangeFace()
    {
        Image.sprite = AllIdolSprites[GameManager.Instance.teamIndex];
    }
}
