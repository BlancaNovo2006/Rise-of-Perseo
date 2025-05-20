using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EstatuaZeus : MonoBehaviour

{
    public Animator animator;
    private bool Activada = false;
    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Activada && collision.CompareTag("Player"))
        {
            Activada = true;
            animator.SetTrigger("Activar");
        }
        else
        {
            Activada = false;
            animator.ResetTrigger("Activar");
        }
    }

}
