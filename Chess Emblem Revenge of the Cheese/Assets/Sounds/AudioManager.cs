using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

public class AudioManager : MonoBehaviour
{
    // https://www.youtube.com/watch?v=rdX7nhH6jdM&ab_channel=RehopeGameshttps://www.youtube.com/watch?v=rdX7nhH6jdM&ab_channel=RehopeGames

    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlaySFX(string name, float Time)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found, try another name.");
        }

        else
        {
            print(s.clip);
            sfxSource.PlayOneShot(s.clip);
            StartCoroutine(StopSound(Time, sfxSource));
        }
    }
    IEnumerator StopSound(float Time, AudioSource Flunk)
    {
        yield return new WaitForSeconds(Time);
        Flunk.Stop();
    }
}
