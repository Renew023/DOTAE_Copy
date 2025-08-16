using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    private bool isBGMStopped = false;
    private Coroutine _bgmFadeCoroutine;
    
    private float _targetBGMVolume => PlayerPrefs.GetFloat("BGMVolume", 0.5f);
    private float _targetSFXVolume => PlayerPrefs.GetFloat("SFXVolume", 0.5f);

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // var instances = FindObjectsOfType<SoundManager>();
        // Debug.LogError($"[SoundManager] 현재 인스턴스 수: {instances.Length}");
        base.Awake();

        DontDestroyOnLoad(gameObject);

        // AudioSource 컴포넌트 추가
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // 저장된 볼륨 설정값을 불러와 적용
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 0.5f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.5f));
        
    }

    private void Start()
    {
        PlayBGMAsync(SoundParameterData.IntroBGM, 1f);
    }

    // 싱글톤 해제하고 사용
    // public void Init()
    // {
    //     // AudioSource 컴포넌트 추가
    //     bgmSource = gameObject.AddComponent<AudioSource>();
    //     sfxSource = gameObject.AddComponent<AudioSource>();
    //
    //     // 저장된 볼륨 설정값을 불러와 적용
    //     SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 0.5f));
    //     SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.5f));
    // }

    private IEnumerator FadeBGM(float fromVolume, float toVolume, float duration, System.Action onComplete = null)
    {
        float time = 0f;
        bgmSource.volume = fromVolume;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(fromVolume, toVolume, time / duration);
            yield return null;
        }

        bgmSource.volume = toVolume;
        onComplete?.Invoke();
    }

    public async void PlayBGMAsync(string bgmKey, float fadeDuration = 0f)
    {
        isBGMStopped = false;

        var clip = /*AddressableManager.Instance._audioCash[bgmKey];*/await AddressableManager.Instance.LoadAudioClip(bgmKey);
        if (clip == null || isBGMStopped) return;
        
        bgmSource.clip = clip;
        bgmSource.loop = true;
        
        if(_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        if (fadeDuration > 0f)
        {
            bgmSource.volume = 0f;
            bgmSource.Play();
            _bgmFadeCoroutine = StartCoroutine(FadeBGM(0f, _targetBGMVolume, fadeDuration));
        }
        else
        {
            bgmSource.volume = _targetBGMVolume;
            bgmSource.Play();
        }
    }

    public void StopBGM(float fadeDuration = 0f)
    {
        isBGMStopped = true;
        
        if(_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        if (fadeDuration > 0f)
        {
            _bgmFadeCoroutine = StartCoroutine(FadeBGM(bgmSource.volume, 0f, fadeDuration, () =>
            {
                bgmSource.Stop();
            }));
        }
        else
        {
            bgmSource.Stop();
        }
    }

    // SFX 1회 재생
    public async void PlaySFXAsync(string sfxKey)
    {
        var clip = /*AddressableManager.Instance._audioCash[sfxKey];*/await AddressableManager.Instance.LoadAudioClip(sfxKey);
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
    

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;

        if (volume > 0f && bgmSource.clip != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // SFX 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}