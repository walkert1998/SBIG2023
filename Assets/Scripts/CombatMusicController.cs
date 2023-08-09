using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMusicController : MonoBehaviour
{
    public static CombatMusicController instance;
    public AudioClip[] combatTracks;
    AudioSource source;
    public bool stopped = false;
    int index = 0;
    // Start is called before the first frame update
    void Awake()
    {
        source = GetComponent<AudioSource>();
        //if (instance != null & instance != this)
        //{
        //    Destroy(instance);
        //}
        instance = this;
    }

    void Start()
    {
        PlayNextTrack();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped && !source.isPlaying)
        {
            PlayNextTrack();
        }
    }

    public static void PlayNextTrack()
    {
        StopMusic();
        instance.source.clip = instance.combatTracks[instance.index];
        PlayMusic();
        instance.index++;
        if (instance.index >= instance.combatTracks.Length - 1)
        {
            instance.index = 0;
        }
    }

    public static void StopMusic()
    {
        instance.stopped = true;
        instance.source.Stop();
    }

    public static void PlayMusic()
    {
        instance.stopped = false;
        instance.source.Play();
    }

    public static void PlaySpecificTrack(AudioClip clip)
    {
        StopMusic();
        instance.source.clip = clip;
        PlayMusic();
    }
}
