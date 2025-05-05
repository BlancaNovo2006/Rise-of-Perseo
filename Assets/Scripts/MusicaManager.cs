using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaManager : MonoBehaviour
{
    public AudioSource musicaNivelCueva;
    public AudioSource musicaBoss;
    public static MusicaManager instance;

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
        if (musicaNivelCueva.isPlaying) musicaNivelCueva.Stop();
        if (!musicaBoss.isPlaying) musicaBoss.Play();
    }

    public void ActivarMusicaNivelCueva()
    {
        if (musicaBoss.isPlaying) musicaBoss.Stop();
        if (!musicaNivelCueva.isPlaying) musicaNivelCueva.Play();
    }
}
