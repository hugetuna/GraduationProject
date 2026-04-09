using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public List<BGMFile> bgmFiles;
    [Header("音效池設定")]
    public int poolSize = 10;
    public AudioSource audioSourcePrefab;
    public GameObject Music;
    public List<AudioSource> playingSFXs;
    private Queue<AudioSource> sfxPool;

    [Header("全域音效音量")]
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        InitializePool();
    }

    private void InitializePool()
    {
        sfxPool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = Instantiate(audioSourcePrefab, transform);
            src.playOnAwake = false;
            sfxPool.Enqueue(src);
        }
    }
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        // 更新正在播放的音效音量
        foreach (var src in playingSFXs)
        {
            src.volume = volume * sfxVolume;
        }
        // 更新背景音樂音量
        AudioSource musicSource = Music.GetComponent<AudioSource>();
        musicSource.volume = volume * musicVolume;
    }
    public void SetSFXVolume(float newSFXVolume)
    {
        sfxVolume = newSFXVolume;
        // 更新正在播放的音效音量
        foreach (var src in playingSFXs)
        {
            src.volume = volume * sfxVolume;
        }
    }
    public void SetMusicVolume(float newMusicVolume)
    {
        musicVolume = newMusicVolume;
        // 更新背景音樂音量
        AudioSource musicSource = Music.GetComponent<AudioSource>();
        musicSource.volume = volume * musicVolume;
    }
    // 播放音效
    public void PlaySFX(AudioClip clip, float setVolume = 1f, float setPitch = 1f)
    {
        if (clip == null) return;

        // 取出可用音源
        AudioSource src = sfxPool.Dequeue();
        src.clip = clip;
        src.volume = volume * sfxVolume * setVolume;
        src.pitch = setPitch;
        src.Play();
        playingSFXs.Add(src);
        // 播完自動放回池中
        StartCoroutine(ReturnToPool(src, clip.length));
    }
    // 停止所有音效
    public void StopSFX()
    {
        // 建立一個臨時列表來處理所有正在播放的音源
        List<AudioSource> stoppedSources = new List<AudioSource>(playingSFXs);

        // 清空 playingSFXs 列表，這是關鍵步驟！
        // 這樣即使 ReturnToPool 協程完成，它也找不到該音源來移除。
        playingSFXs.Clear();

        // 將每個音源停止並放回池中
        foreach (var src in stoppedSources)
        {
            // 停止播放
            src.Stop();
            sfxPool.Enqueue(src);
        }
    }
    // 設定背景音樂
    public void SetMusic(AudioClip clip, float setVolume = 1f)
    {
        AudioSource musicSource = Music.GetComponent<AudioSource>();
        musicSource.clip = clip;
        musicSource.volume = volume * musicVolume * setVolume;
        musicSource.loop = true;
        musicSource.Play();
    }
    //停止背景音樂
    public void StopMusic()
    {
        AudioSource musicSource = Music.GetComponent<AudioSource>();
        musicSource.Stop();
    }
    // 將音源放回池中
    private IEnumerator ReturnToPool(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playingSFXs.Contains(src))
        {
            playingSFXs.Remove(src);
            src.Stop(); // 預防性停止
            sfxPool.Enqueue(src);
        }
    }
}
