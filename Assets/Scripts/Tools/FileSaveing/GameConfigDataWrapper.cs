using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigDataWrapper
{
    // 音量設定
    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;
    //畫面設定
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public FullScreenMode fullScreenMode = FullScreenMode.Windowed;
    //按鍵設定
}
