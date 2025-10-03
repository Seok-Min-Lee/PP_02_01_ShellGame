using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public struct Sound
{
    public enum Key
    {
        Bgm,
        BgmSfx,
        Click,
        Correct,
        Defeat,
        Go,
        Next,
        Ready,
        Timer,
        Victory,
        Wrong,
    }
    public Sound(Key key, AudioClip audioClip)
    {
        this.key = key; 
        this.audioClip = audioClip;
    }
    public Key key { get; private set; }
    public AudioClip audioClip { get; private set; }
}

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource sourceBGM;
    [SerializeField] private AudioSource sourceSFX;

    public bool isLoadComplete { get; private set; }

    // 불러온 오디오를 Sound 구조체로 변환하여 딕셔너리에 담는다.
    // Key: Sound.key, Value: Sound
    public Dictionary<Sound.Key, Sound> soundDictionary = new Dictionary<Sound.Key, Sound>();

    public void Load(Action callback = null)
    {
        // Resources 폴더를 사용하는 경우
        Sound.Key[] keys = (Sound.Key[])Enum.GetValues(typeof(Sound.Key));

        foreach (var key in keys)
        {
            Sound sound = new Sound(
                key: key,

                audioClip: Resources.Load<AudioClip>($"Audio/{key}")
            );

            soundDictionary.Add(key, sound);
        }

        isLoadComplete = true;

        callback?.Invoke();
    }
    public void Init(float volumeBGM, float volumeSFX)
    {
        sourceBGM.volume = volumeBGM;
        sourceSFX.volume = volumeSFX;
    }
    public void PlayBGM(Sound.Key key)
    {
        if (!sourceBGM.isPlaying && 
            soundDictionary.TryGetValue(key, out Sound sound))
        {
            sourceBGM.clip = sound.audioClip;
            sourceBGM.Play();
        }
    }
    public void StopBGM()
    {
        sourceBGM.Stop();
    }
    public void PlaySFX(Sound.Key key)
    {
        if (soundDictionary.TryGetValue(key, out Sound sound))
        {
            sourceSFX.PlayOneShot(sound.audioClip);
        }
    }
}