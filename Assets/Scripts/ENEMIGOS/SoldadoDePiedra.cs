using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoldadoDePiedra : ControladorEnemigos
{
    public Collider2D espadaPiedraCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        Atacando = GetComponent<ControladorEnemigos>();
        EnMovimiento = GetComponent<ControladorEnemigos>();
        movement = GetComponent<Vector2>();

        DesactivarEspadaCollider();
    }

    protected void Update()
    {
        base.Update();
        AtaqueEnemigo();
        animator.SetBool("Atacando", Atacando);
    }
    
    void AtaqueEnemigo()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRadius)
        {
            if (!Atacando)
            {
                Atacando = true;
                EnMovimiento = false;
                movement = Vector2.zero;
            }
        }
        else
        {
            Atacando = false;
            DesactivarEspadaCollider();
        }
    }
    public void ActivarEspadaCollider()
    {
        espadaPiedraCollider.enabled = true;
    }
    public void DesactivarEspadaCollider()
    {
        espadaPiedraCollider.enabled = false;
    }
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}