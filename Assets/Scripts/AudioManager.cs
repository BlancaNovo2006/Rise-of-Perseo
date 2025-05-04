using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }
    private AudioSource AudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("¡Más de 1 AudioManager en escena!");
        }
    }
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void ReporducirSonido(AudioClip audio)
    {
        AudioSource.PlayOneShot(audio);
    }
}