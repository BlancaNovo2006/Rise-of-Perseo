using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminarMusicaMar : MonoBehaviour
{
    void Start()
    {
        MusicaManagerAgua mm = FindObjectOfType<MusicaManagerAgua>();
        if (mm != null)
        {
            Destroy(mm.gameObject);
        }
    }
}