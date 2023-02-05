using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds;
    public AudioSource musicSource;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        PlayMusic("powerup");
    }
    
    public void PlayMusic(string name) {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if(s != null) {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
}
