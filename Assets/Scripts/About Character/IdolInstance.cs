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
    public int charm;//�y�O
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
        charm = basicStatus.charm;
        performance = basicStatus.performance;
        vigour = vigourMax = basicStatus.vigour;
        fans = 0;
    }
    
    

}
