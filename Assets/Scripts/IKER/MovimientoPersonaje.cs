using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
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
    public bool muerto;
    private bool atacando;

    public float velocidadDeMovimientoBase;
    public float velocidadExtra;
    public float tiempoSprint;
    private float tiempoActualSprint;
    private float tiempoSiguienteSprint;

    public float tiempoEntreSprints;
    private bool puedeCorrer = true;
    private bool estaCorriendo = false;

    private Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tiempoActualSprint = tiempoSprint;
    }

    void Update()
    {
        if (!muerto)
        {
            if (!recibiendoDanio)
            {
                if (!atacando)
                {
                    ProcesarMovimiento();
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
                    enSuelo = hit.collider != null;

                    //Salto
                    if (enSuelo && Input.GetKeyDown(KeyCode.Space))// && !RecibiendoDaño)
                    {
                        rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                    }
                    //Planear
                    if (!enSuelo && Input.GetKey(KeyCode.Space))
                    {
                        if (rb.velocity.y <= 0f)
                        {
                            rb.gravityScale = 0.25f;
                        }

                    }
                    else
                    {
                        rb.gravityScale = 1f;
                    }
                    //Correr
                    if (Input.GetKeyDown(KeyCode.B) && puedeCorrer)
                    {
                        velocidad = velocidadExtra;
                        estaCorriendo = true;
                    }
                    if (Input.GetKeyUp(KeyCode.B))
                    {
                        velocidad = velocidadDeMovimientoBase;
                        estaCorriendo = false;
                    }
                    if (Mathf.Abs(rb.velocity.x) >= 0.1f && estaCorriendo)
                    {
                        if(tiempoActualSprint > 0)
                        {
                            tiempoActualSprint -= Time.deltaTime;
                        }
                        else
                        {
                            velocidad = velocidadDeMovimientoBase;
                            estaCorriendo = false;
                            puedeCorrer = false;
                            tiempoSiguienteSprint = Time.time + tiempoEntreSprints;
                        }
                    }
                    if (!estaCorriendo && tiempoActualSprint <= tiempoSprint && Time.time >= tiempoSiguienteSprint)
                    {
                        tiempoActualSprint += Time.deltaTime;
                        if (tiempoActualSprint >= tiempoSprint)
                        {
                            puedeCorrer = true;
                        }
                    }
                }
                //Atacar
                if (Input.GetKeyDown(KeyCode.Z) && !atacando && enSuelo)
                {
                    Atacando();
                }
            }
        }
        
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("Atacando", atacando);
    }

    void ProcesarMovimiento()
    {
        //Logica de movimiento
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(horizontal, 0) * velocidad * Time.deltaTime;

        animator.SetFloat("movement", horizontal * velocidad);

        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 0);
        }
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 0);
        }

        transform.position = transform.position + dir;
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            if (vida <= 0)
            {
                muerto = true;
            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivarDanio());
            }
        }
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
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
