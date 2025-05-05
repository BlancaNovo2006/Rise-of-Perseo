using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {  get; private set; }
    private AudioSource AudioSource;

    /*private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("¡Más de 1 AudioManager en escena!");
        }
    }*/
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
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void ReporducirSonido(AudioClip audio)
    {
        AudioSource.PlayOneShot(audio);
    }
}