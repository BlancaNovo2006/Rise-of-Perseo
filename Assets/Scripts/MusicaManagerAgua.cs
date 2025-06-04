using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaManagerAgua : MonoBehaviour
{
    public AudioSource musicaNivelMar;
    public AudioSource musicaBoss;
    public static MusicaManagerAgua instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void ActivarMusicaBoss()
    {
        if (musicaNivelMar.isPlaying) musicaNivelMar.Stop();
        if (!musicaBoss.isPlaying) musicaBoss.Play();
    }

    public void ActivarMusicaNivelMar()
    {
        if (musicaBoss.isPlaying) musicaBoss.Stop();
        if (!musicaNivelMar.isPlaying) musicaNivelMar.Play();
    }
}
