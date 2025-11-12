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
    // 播放音效
    public void PlaySFX(AudioClip clip, float setVolume = 1f)
    {
        if (clip == null) return;

        // 取出可用音源
        AudioSource src = sfxPool.Dequeue();
        src.clip = clip;
        src.volume = volume * sfxVolume * setVolume;
        src.Play();

        // 播完自動放回池中
        StartCoroutine(ReturnToPool(src, clip.length));
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
        sfxPool.Enqueue(src);
    }
}

