using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorriorFaceChange : MonoBehaviour
{
    [SerializeField]
    public List<Image> Images;//需要更换表情的Image组件列表
    public List<Sprite> AllIdolSprites;//所有偶像的表情圖
    void Start()
    {
        ChangeFace();
    }
    public void ChangeFace()
    {
        for (int i = 0; i < Images.Count; i++)
        {
            Image image = Images[i];
            int targetIndex = (int)GameManager.Instance.idolDataList[i].idolIndex;
            image.sprite= AllIdolSprites[targetIndex];
        }
    }
}
