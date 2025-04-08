using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour, IInteractable
{
    public bool isPlantable = false; // �O�_�i�H�ش�
    public bool isPlanting = false;//�O�_�b�شӤ�
    public Transform seedSpawnPoint; // �ؤl���ͦ���m
    public float yOffSet=1.5f;
    public GameObject[] seedPrefabs; // �x�s���P�������ؤl�w�s��
    public SeedInstanceScript seedOnThisSoil;//�x�s�@�ӳQ�شӪ��ؤl���ƥ�
    //½�g
    public void TurnTheSoil()
    {
        if(isPlanting == false && isPlantable == false)
        {
            isPlantable = true;
            Debug.Log("½�g!");
        }
        else
        {
            Debug.Log("���a���w½�L�g�Υ��b�ش�");
        }
    }
    //�ؤ@�Ӻؤl
    public void PlantSeed(int seedIndex)
    {
        if (!isPlantable)
        {
            Debug.Log("�o���g�a�٤���شӡI");
            return;
        }
        if (seedIndex < 0 || seedIndex >= seedPrefabs.Length)
        {
            Debug.Log("�L�Ī��ؤl�����I");
            return;
        }
        // �b���w��m�ͦ��ؤl
        Vector3 realSpawnPoint = seedSpawnPoint.position + new Vector3(0, yOffSet, 0);
        Quaternion rotation = Quaternion.Euler(45f, 0f, 0f);
        seedOnThisSoil =Instantiate(seedPrefabs[seedIndex], realSpawnPoint, rotation).GetComponent<SeedInstanceScript>();
        //��j�p
        //seedOnThisSoil.transform.localScale = seedOnThisSoil.transform.localScale * 0.5f;
        isPlantable = false; // �аO�o���g�a�w�g�Q�ش�
        isPlanting = true;
    }
    void IInteractable.Interact(int toolType) // ���ʦ欰
    {
        //TODO:�l�[�u��ϬM
        //���B��شӪ��A�B���i�ش�->½�g(�ϥξS�Ytool==1)
        if(isPlanting == false && isPlantable == false && toolType==1)
        {
            TurnTheSoil();
        }
        //���B��شӪ��A�B�i�ش�->�ش�
        else if (isPlanting == false && isPlantable == true)
        {
            PlantSeed(1);
        }
        //�شӤ��A����������->���
        else if (isPlanting == true && isPlantable == false && seedOnThisSoil.GetDaysGrown()< seedOnThisSoil.seedData.growthDays)
        {
            seedOnThisSoil.GetComponent<SeedInstanceScript>().Water();
        }
        //�شӤ��A�������H��->����
        else if (isPlanting == true && isPlantable == false && seedOnThisSoil.GetDaysGrown() == seedOnThisSoil.seedData.growthDays)
        {
            seedOnThisSoil.GetComponent<SeedInstanceScript>().Harvest();
            isPlanting = false;
            isPlantable = false;
        }
    } 
}
