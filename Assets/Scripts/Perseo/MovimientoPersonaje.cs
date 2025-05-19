using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class MovimientoPersonaje : MonoBehaviour
{
    SistemaHabilidades sistemaHabilidades;

    public int vida = 10;
    public float velocidad = 7f;
    public float fuerzaSalto = 7f;
    public float fuerzaRebote = 6f;
    public float longitudRaycast = 0.3f;
    public LayerMask capaSuelo;
    private SpriteRenderer spriteRenderer;
    private int enemyLayer;
    
    public LayerMask Enemy;

    private bool enSuelo = true;
    private bool recibiendoDanio = false;
    public bool muerto = false;
    public bool atacando = false;
    private bool caminar =false;
    private bool planeando = false;
    private bool enDash = false;

    public int experienciaActual = 0;
    public TextMeshProUGUI contadorExperiencia;
    //Recuperar Experiencia
    //public Vector2 ultimaPosicionMuerte;
    //public int experienciaPerdida = 0;

    public Transform puntoRespawn;
    public float tiempoRespawn = 0.6f;

    public float tiempoRodar = 0.5f;
    public float velocidadRodar = 10f;
    private bool rodando = false;
    public bool invencible = false;

    private Rigidbody2D rb;
    public Animator animator;

    private float coyoteTimeCounter;
    public float coyoteTimeDuration = 0.2f;

    private Vector2 ultimoPuntoRespawn;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sistemaHabilidades = GetComponent<SistemaHabilidades>();
        ultimoPuntoRespawn = puntoRespawn.position;

        ActualizarUIExperiencia();

        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyLayer = LayerMask.NameToLayer("Enemy");
        if (spriteRenderer == null)
        {
            Debug.LogError("El objeto no tiene un SpriteRenderer asignado");
        }
    }

    void Update()
    {
        if (!muerto)
        {
            if (!recibiendoDanio)
            {
                if (!rodando && !atacando)
                {
                    ProcesarMovimiento();
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);

                    if (Input.GetKeyDown(KeyCode.LeftShift) && enSuelo)
                    {
                        StartCoroutine(Rodar());
                    }

                    enSuelo = hit.collider != null;

                    
                }
                //CoyoteTime
                if (enSuelo)
                {
                    coyoteTimeCounter = coyoteTimeDuration;
                }
                else
                {
                    coyoteTimeCounter -= Time.deltaTime;
                }

                //Salto
                if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f && !atacando)
                {
                    if (rodando)
                    {
                        rodando = false;
                        enDash = false;
                        invencible = false;
                        animator.SetBool("endash", false); // Para evitar conflicto de animaciones
                    }
                    rb.velocity = new Vector2(0, fuerzaSalto);
                    coyoteTimeCounter = 0f; // Evita que se pueda saltar varias veces en el aire
                }
                //Planear
                if (!enSuelo && Input.GetKey(KeyCode.Space))
                {
                    if (rb.velocity.y <= 0f)
                    {
                        rb.gravityScale = 0.25f;
                    }
                    planeando = true;
                }
                else
                {
                    rb.gravityScale = 1f;
                    planeando = false;
                }
                //Atacar
                if (Input.GetKeyDown(KeyCode.Mouse0) && !atacando && enSuelo)
                {
                    Atacando();
                } 
            }
            if (!recibiendoDanio && Mathf.Abs(rb.velocity.x) > 0.1f && enSuelo && !rodando && !atacando)
            {
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0f, Time.deltaTime * 5f), rb.velocity.y);
            }

        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("Caminar", caminar);
        //animator.SetBool("Rodando", rodando);
        animator.SetBool("recibiendoDanio", recibiendoDanio);
        animator.SetBool("muelto", muerto);
        animator.SetBool("planeando", planeando);
        animator.SetBool("endash", enDash);
    }
    
    IEnumerator Rodar()
    {
        rodando = true;
        enDash = true;
        invencible = true;
        animator.SetBool("endash", true);

        // Ignorar colisión entre Jugador y Enemigos
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        float direccion = transform.localScale.x; // 1 si mira derecha, -1 si mira izquierda
        rb.velocity = new Vector2(velocidadRodar * direccion, rb.velocity.y);

        yield return new WaitForSeconds(tiempoRodar);

        rb.velocity = Vector2.zero;  // Detiene el deslizamiento

        // Restaurar colisión entre Jugador y Enemigos
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        invencible = false;
        rodando = false;
        enDash = false;
    }
    
    void ProcesarMovimiento()
    {
        //Logica de movimiento
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(horizontal, 0) * velocidad * Time.deltaTime;

        caminar = horizontal != 0;

        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 0);
        }
        if (horizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 0);
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, longitudRaycast/2.5f, capaSuelo);
        if (hit.collider == null)
        {
            Vector2 spd = rb.velocity;
            spd.x = 0;
            rb.velocity = spd;
            transform.position = transform.position + dir;
        }
        
    }
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (invencible) return; // Si es invencible, no recibe daño
        animator.SetBool("recibiendoDanio", true);

        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            atacando = false;
            animator.SetBool("Atacando", false);
            if (vida <= 0)
            {
                muerto = true;
                animator.SetBool("muelto", true);
                StartCoroutine(Respawnear());
            }
            if (!muerto)
            {
                spriteRenderer.color = Color.red;
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivarDanio());
            }
        }
    }
    IEnumerator Respawnear()
    {
        yield return new WaitForSeconds(tiempoRespawn);
        transform.position = ultimoPuntoRespawn;
        vida = 10;
        muerto = false;
        recibiendoDanio = false;
        animator.SetBool("muelto", false);
        animator.SetBool("recibiendoDanio", false);
        rb.velocity = Vector2.zero;
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.4f);
        spriteRenderer.color = Color.white;
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("recibiendoDanio", false);

    }
    public void Atacando()
    {
        atacando = true;
        animator.SetBool("Atacando", true);
    }
    public void DesactivarAtaque()
    {
        atacando = false;
        animator.SetBool("Atacando", false);
    }
    
    
    
    public void AgregarExperiencia(int cantidad)
    {
        experienciaActual += cantidad;
        ActualizarUIExperiencia();
    }
    void ActualizarUIExperiencia()
    {
        if(contadorExperiencia != null)
        {
            contadorExperiencia.text = "XP:" + experienciaActual;
        }
    }
    
    

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EspadaSoldado"))
        {
            Debug.Log("atacado por enemigo");
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
        }

        if (collision.CompareTag("Proyectil"))
        {
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
            
        }

        if (collision.CompareTag("ColaMedusa"))
        {
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);

        }
        if (collision.CompareTag("Respawn"))
        {
            StartCoroutine(Respawnear());
        }
        if (collision.CompareTag("PuntoRespawn"))
        {
            ultimoPuntoRespawn = collision.transform.position;
            Debug.Log("Nuevo punto de respawn activado");
            vida = 10;
            sistemaHabilidades.vialRegenerativo = 5;
            sistemaHabilidades.ActualizarUIVidas();
        }
    }
}
