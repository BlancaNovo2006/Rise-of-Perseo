using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminarMusicaMenu : MonoBehaviour
{
    void Start()
    {
        MusicMenuManager mm = FindObjectOfType<MusicMenuManager>();
        if (mm != null)
        {
            Destroy(mm.gameObject);
        }
    }
}