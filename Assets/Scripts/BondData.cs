using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] // �����i�Q Unity �ǦC�ơ]Inspector �i�վ�ƭȡ^
public class BondData//�ΥH�x�s���̭Ȫ���
{
    public IdolInstance idolA;  // ����A
    public IdolInstance idolB;  // ����B
    [Range(0, 100)] public int bondValue; // ���̭� (0~100)
}
