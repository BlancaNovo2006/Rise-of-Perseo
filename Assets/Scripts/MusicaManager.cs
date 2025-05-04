using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaManager : MonoBehaviour
{
    public AudioSource musicaNivelCueva;
    public AudioSource musicaBoss;

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
