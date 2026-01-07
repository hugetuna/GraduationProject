using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetVolumeUI : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI volumeText;
    public enum VolumeType { Master, Music, SFX }
    public VolumeType volumeType;
    void Start()
    {
        Initialize();
    }
    public void HandleValueChanged(float value)
    {
        volumeText.text= Mathf.Floor(value * 100).ToString();
    }
    public void Initialize()
    {
        // 根據音量類型設置滑桿初始值
        if (volumeType == VolumeType.Master)
            Slider.value = AudioManager.Instance.volume;
        else if (volumeType == VolumeType.Music)
            Slider.value = AudioManager.Instance.musicVolume;
        else if (volumeType == VolumeType.SFX)
            Slider.value = AudioManager.Instance.sfxVolume;
        // 設置初始文字顯示
        volumeText.text = Mathf.Floor(Slider.value * 100).ToString();
    }
}
