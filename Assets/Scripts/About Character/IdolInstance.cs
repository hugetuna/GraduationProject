using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdolInstance : MonoBehaviour
{
    //���⪺��¦�ƭ�
    public Idols basicStatus;
    //��ڹB�檺�ƭ�
    public int vocal;//�q�ۤO
    public int dance;//�R�ФO
    public int visual;//��{�O
    //�T�ؽm�߮Ĳv
    public float voTrainingBonus;
    public float daTrainingBonus;
    public float viTrainingBonus;
    public int charm;//��¦�y�O
    public int charmInCount;//�y�O->�ȮɤW�ɩΤU���᪺�ƭȡA����]�γo��
    public int performance;//�t��
    public int vigour;//��O
    public int vigourMax;//��O�̤j��
    //������
    public int fans;
    public int bondWithP;//�P���a������
    // Start is called before the first frame update
    void Start()
    {
        IdolSetUp();
    }
    
    //��l��
    private void IdolSetUp()
    {
        if (basicStatus == null)
        {
            Debug.LogError("�򥻪��A (basicStatus) ���]�w�I");
            return;
        }
        vocal = basicStatus.vocal;
        dance = basicStatus.dance;
        visual = basicStatus.visual;
        voTrainingBonus=1f;
        daTrainingBonus=1f;
        viTrainingBonus=1f;
        charm = basicStatus.charm;
        charmInCount = charm;
        performance = basicStatus.performance;
        vigour = vigourMax = basicStatus.vigour;
        fans = 0;
    }
    //�C�ѵ����ɥ������s�����Ȯɪ��A
    public void resetTemporaryEffect()
    {
        charmInCount = charm;
        voTrainingBonus = 1f;
        daTrainingBonus = 1f;
        viTrainingBonus = 1f;
    }
    public void FansIncrease(int increseAmount)
    {
        fans += increseAmount;
    }
}
