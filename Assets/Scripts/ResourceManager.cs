using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public int Money;
    public float MoneyBonus=1f;//�ȿ����v
    public BondData bondAB;//�C���AA�PB�����̭ȡA�H�U�P
    public BondData bondBC;
    public BondData bondCA;
    public List<IdolInstance> idolsPicked;//��i����T�W����
    public List<Item> items;
    //�C�ѵ����ɥ������s�귽�Ȯɪ��A
    public void resetTemporaryEffect()
    {
        MoneyBonus = 1f;
    }
}
