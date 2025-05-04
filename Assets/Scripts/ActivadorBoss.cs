using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorBoss : MonoBehaviour
{
    private MusicaManager musicaManager;

    private void Start()
    {
        musicaManager = FindObjectOfType<MusicaManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            musicaManager.ActivarMusicaBoss();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            musicaManager.ActivarMusicaNivelCueva();
        }
    }
}
