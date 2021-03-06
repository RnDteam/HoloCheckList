﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceManager : MonoBehaviour {

    public AudioSource[] Sounds;

    void Start () {
        Sounds = GetComponents<AudioSource>();	
	}

    public void PlayAudio(int i)
    {
        if (i >= 0 && i <= Sounds.Length)
        {
            Sounds[i].Play();
        }
    }

    public void StopAll()
    {
        foreach (AudioSource aSource in Sounds)
        {
            aSource.Stop();
        }
    }

    public void PlayNoTasks()
    {
        Sounds[Sounds.Length - 1].Play();
    }
}
