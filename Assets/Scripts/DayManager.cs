using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static int date = 1; // �R�A�ܼơA�O�s�C���������

    // �Ψӧ�ʤ�����禡
    public void ChangeDay()
    {
        date++;
        Debug.Log($"���ѬO�� {date} ��");
    }

    // �i�H�b�}�l�C���ɪ�l�ơA�ΨC���C�������ɶi��O�s
    void Start()
    {
        // ��l�Ƥ���A�Y�ݭn�i�H�令�q�O�s��Ū��
        date = 1;
    }
}
