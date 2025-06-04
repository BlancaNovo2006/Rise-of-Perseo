using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorBoss : MonoBehaviour
{
    private MusicaManagerAgua musicaManagerAgua; 

    private void Start()
    {
        musicaManagerAgua = FindObjectOfType<MusicaManagerAgua>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            musicaManagerAgua.ActivarMusicaBoss();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            musicaManagerAgua.ActivarMusicaNivelMar();
        }
    }
}
