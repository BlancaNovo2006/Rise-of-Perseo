using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheets : MonoBehaviour
{
    public Transform CheetTp1;
    public Transform CheetTp2;
    public Transform CheetTpSpawn;
    private Vector2 posicionTp1;
    private Vector2 posicionTp2;
    private Vector2 posicionTpSpawn;

    MovimientoPersonaje moviemientoPersonaje;

    void Start()
    {
        posicionTp1 = CheetTp1.position;
        posicionTp2 = CheetTp2.position;
        posicionTpSpawn = CheetTpSpawn.position;
        moviemientoPersonaje = GetComponent<MovimientoPersonaje>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            transform.position = posicionTp1;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            transform.position = posicionTp2;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            transform.position = posicionTpSpawn;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            moviemientoPersonaje.invencible = !moviemientoPersonaje.invencible;
        }
    }
}
