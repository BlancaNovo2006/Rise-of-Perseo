using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    public int vida = 3;

    public float velocidad;
    public float fuerzaSalto = 10f;
    public float fuerzaRebote = 6f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;

    private bool enSuelo;
    private bool recibiendoDanio;
    private bool muerto;
    private bool atacando;

    private Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        if (!muerto)
        {
            ProcesarMovimiento();
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
            enSuelo = hit.collider != null;

            if (enSuelo && Input.GetKeyDown(KeyCode.Space))// && !RecibiendoDaño)
            {
                rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.Z) && !atacando && enSuelo)
            {
                Atacando();
            }
        }
        

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("Atacando", atacando);
    }

    void ProcesarMovimiento()
    {
        //Logica de movimiento
        float inputMovimiento = Input.GetAxis("Horizontal");
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector2(inputMovimiento * velocidad, rigidbody.velocity.y);
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            
            if (vida<=0)
            {
                muerto = true;
            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    public void Atacando()
    {
        atacando = true;
    }

    public void DesactivarAtaque()
    {
        atacando = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

}
