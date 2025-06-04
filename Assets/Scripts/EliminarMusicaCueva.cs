using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliminarMusicaCueva : MonoBehaviour
{
    void Start()
    {
        MusicaManager mm = FindObjectOfType<MusicaManager>();
        if (mm != null)
        {
            Destroy(mm.gameObject);
        }
    }
}