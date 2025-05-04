using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Detectar si se presiona Shift izquierdo
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            RodarJugador();
        }
    }

    void RodarJugador()
    {
        anim.SetTrigger("rodarplayer");
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }

    // Esta función debe llamarse al final de la animación con un Animation Event
    public void RestaurarGravedad()
    {
        rb.useGravity = true;
    }
}