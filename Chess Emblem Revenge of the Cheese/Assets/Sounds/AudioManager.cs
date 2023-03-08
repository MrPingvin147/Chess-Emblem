using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // https://www.youtube.com/watch?v=rdX7nhH6jdM&ab_channel=RehopeGameshttps://www.youtube.com/watch?v=rdX7nhH6jdM&ab_channel=RehopeGames

    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found, try another name.");
        }

        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }
}
