using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGMFile
{
    public string BGMName;
    public AudioClip audioClip;
}
public class BGMPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public List<BGMFile> bgmFiles;
    public void SetAndPlayBGM(string TargetBGMTag)
    {
        BGMFile hitFile= bgmFiles.Find(file => file.BGMName == TargetBGMTag);
        audioSource.clip = hitFile.audioClip;
        audioSource.Play();
    }
}
