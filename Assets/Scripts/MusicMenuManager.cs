using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMenuManager : MonoBehaviour
{
    private static MusicMenuManager instancia;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Evita duplicados si ya existe uno
        }
    }
}
