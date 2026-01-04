using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MemberConfig : MonoBehaviour
{
    public Image MemberPhoto;
    public int MemberIDNow=-1;
    public Sprite EmptyPhoto;
    public List<Sprite> IdolPhotos;
    public void SetMemberPhoto(int memberID)
    {
        if (memberID < 0 || memberID >= IdolPhotos.Count)
        {
            MemberPhoto.sprite = EmptyPhoto;
        }
        else
        {
            MemberPhoto.sprite = IdolPhotos[memberID];
        }
        MemberIDNow = memberID;
    }
    public void Dispick()
    {
        if (MemberIDNow==-1) return;
        FindAnyObjectByType<PickManager>().pick(MemberIDNow);
    }
}
