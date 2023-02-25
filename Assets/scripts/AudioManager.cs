using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource src;
    public AudioSource musicSrc;
    public static AudioManager Instance;
    void Awake()
    {
        if(Instance==null)
        {
            Instance=this;
            DontDestroyOnLoad(this.gameObject);
        }  
    }
    public void PlaySound(AudioClip clip)
    {
        src.clip=clip;
        src.Play();
    }
    public void PlayMusic(AudioClip clip)
    {
        musicSrc.clip=clip;
        musicSrc.Play(); 
    }
    public void StopMusic()
    {
        musicSrc.Stop();
        src.Stop();
    }
}
