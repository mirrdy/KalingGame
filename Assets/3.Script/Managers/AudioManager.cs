using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Clip")]
    public Sound[] bgm;
    public Sound[] sfx;

    [Space(10f)]
    [Header("Audio Source")]
    [SerializeField] public AudioSource bgmPlay;
    [SerializeField] public AudioSource[] sfxPlay;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AudioSetting();
    }

    private void AudioSetting()
    {
        bgmPlay = transform.GetChild(0).GetComponent<AudioSource>();
        sfxPlay = transform.GetChild(1).GetComponents<AudioSource>();
        PlayerBGM("Title");
    }

    public void PlayerBGM(string name)
    {
        foreach (Sound s in bgm)
        {
            if (s.name.Equals(name))
            {
                bgmPlay.clip = s.clip;
                bgmPlay.Play();
                break;
            }
        }
    }

    public void StopBGM()
    {
        bgmPlay.Stop();
    }

    public void PlaySFX(string name)
    {
        foreach (Sound s in sfx)
        {
            if (s.name.Equals(name))  // clip¿ª √£∞Ì 
            {
                for (int i = 0; i < sfxPlay.Length; i++)
                {
                    if (!sfxPlay[i].isPlaying)
                    {
                        sfxPlay[i].clip = s.clip;
                        sfxPlay[i].Play();
                        return;
                    }
                }
                return;
            }
        }
    }
}
