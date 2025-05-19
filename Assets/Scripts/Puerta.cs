using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puerta : MonoBehaviour
{
    public GameObject puerta;
    private bool abierta = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!abierta && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            AbrirPuerta();
        }
    }
    void AbrirPuerta()
    {
        Debug.Log("!Todos los enemigos estan muertos! Puerta abierta.");
        abierta = true;
        puerta.SetActive(false);
    }
}
