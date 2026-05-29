using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IdolInClothChange : MonoBehaviour,IDropHandler
{
    public IdolWho idolWhoInClothChange;
    public IdolInstance idolInstanceInClothChange;
    public Image ImageInClothChange; // 用來顯示衣服的圖片
    public Sprite Nothing; // 預設的空白圖片
    public List<Sprite> KumaClothSprites = new List<Sprite>();
    public List<Sprite> KaroClothSprites = new List<Sprite>();
    public List<Sprite> SiriusClothSprites = new List<Sprite>();
    public List<Sprite> MizarClothSprites = new List<Sprite>();
    public List<Sprite> AicorClothSprites = new List<Sprite>();
    public void SetIdolWhoInClothChange(IdolWho idolWho)
    {
        idolWhoInClothChange = idolWho;
    }
    public void ChangeCloth(int clothIndex)
    {
        switch (idolWhoInClothChange)
        {
            case IdolWho.none:
                ImageInClothChange.sprite = Nothing;
                break;
            case IdolWho.Kuma:
                ImageInClothChange.sprite = KumaClothSprites[clothIndex];
                break;
            case IdolWho.Karo:
                ImageInClothChange.sprite = KaroClothSprites[clothIndex];
                break;
            case IdolWho.Sirius:
                ImageInClothChange.sprite = SiriusClothSprites[clothIndex];
                break;
            case IdolWho.Mizar:
                ImageInClothChange.sprite = MizarClothSprites[clothIndex];
                break;
            case IdolWho.Aicor:
                ImageInClothChange.sprite = AicorClothSprites[clothIndex];
                break;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("換裝");
        if (eventData.pointerDrag != null)
        {
            DragableCloth item = eventData.pointerDrag.GetComponent<DragableCloth>();
            if (item != null)
            {
                ChangeCloth(item.clothIndex);
                //將衣服的索引傳給偶像實例，讓它知道自己換了哪套衣服
                idolInstanceInClothChange.ChangeCloth(item.clothIndex);
            }
        }
    }
}
