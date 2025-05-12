using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargolaDePiedra : MonoBehaviour
{
    public Transform player;
    public float detectionRadius;
    public float attackRadius;
    public float speed;
    public float fuerzaRebote;
    public int vidas;  // Vidas del enemigo

    public GameObject experienciaPrefab;
    public GameObject ProyectilPrefab;
    public float velocidadProyectil;
    public int experienciaSoltar = 20;

    protected Rigidbody2D rb;
    protected Vector2 movement;
    protected bool playervivo;
    protected bool muerto;
    protected bool recibiendoDanio;
    protected bool Atacando;

    protected bool canseePlayer = true;

    protected bool isFrozen = false;
    protected float originalSpeed;
    protected SpriteRenderer spriteRenderer;

    protected Animator animator;

    public Material grayscaleMaterial;
    private Material originalMaterial;
    
    protected bool Petrificado = false;

    void Start()
    {
        playervivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpeed = speed;
    }

    protected void Update()
    {
        if (player != null && playervivo && !muerto && !isFrozen)
        {
                Movimiento();
                AtaqueEnemigo();
                if (transform.position == player.position)
                {
                    movement = Vector2.zero;
                }
            
        }
        if (!playervivo)
        {
            movement = Vector2.zero;
        }
    }
    protected void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            if (!Atacando)
            {
                if (direction.x < 0)
                {
                    transform.localScale = new Vector3(1, 1, 0);
                }
                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(-1, 1, 0);
                }
            }
            movement = new Vector2(direction.x, direction.y);
        }
        else
        {
            movement = Vector2.zero;
            Atacando = false;
        }

        if (distanceToPlayer <= attackRadius)
        {
            movement = Vector2.zero;
        }
    }
    void FixedUpdate()
    {
        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }
    protected void AtaqueEnemigo()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRadius)
        {
            if (!Atacando)
            {
                Atacando = true;
                animator.SetBool("Atacando", Atacando);
            }
        }
        else if (distanceToPlayer > attackRadius)
        {
            Atacando = false;
            animator.SetBool("Atacando", false);
        }
    }

    protected void LanzarAtaque()
    {
        if (player ==  null) return;
        if (ProyectilPrefab == null)
        {
            Debug.LogError("ProyectilPrefab no asignado.");
            return;
        }
        Debug.Log("ataquelanzado");
        Vector2 direccion = (player.position - transform.position).normalized;

        Vector2 spawnPos = (Vector2)transform.position + direccion * 0.5f;
        GameObject Proyectil = Instantiate(ProyectilPrefab, spawnPos, Quaternion.identity);
        Proyectil.GetComponent<Rigidbody2D>().velocity = direccion * velocidadProyectil;


        if (Proyectil != null)
        {
            Rigidbody2D rbProyectil = Proyectil.GetComponent<Rigidbody2D>();
            if (rbProyectil != null)
            {
                rbProyectil.velocity = direccion * velocidadProyectil;
            }

            // Rotar el proyectil para que mire hacia el jugador
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            Proyectil.transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Debug.Log("golpe con espada");
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
        }
        if (collision.CompareTag("Hoz"))
        {
            Debug.Log("Golpe con Hoz (ataque fuerte)");
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 2);
        }
        if (collision.CompareTag("Pegaso"))
        {
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
        }
    }
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            animator.SetBool("recibiendoDanio", true);

            // Reducir las vidas del enemigo
            vidas -= cantDanio;

            Atacando = false;
            animator.SetBool("Atacando", false);
            animator.Play("Idle", 0);

            // Si las vidas son 0 o menos, destruir al enemigo
            if (vidas <= 0)
            {
                Muerte();
            }
            else
            {
                // Si no ha muerto, aplicar el rebote
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }


            StartCoroutine(DesactivarDanio());
        }
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.25f);
        recibiendoDanio = false;
        animator.SetBool("recibiendoDanio", false);
    }

    protected void Muerte()
    {
        muerto = true;
        //if (experienciaPrefab != null)
        {
            //Vector3 posicion = transform.position;
            //GameObject Experiencia = Instantiate(experienciaPrefab, posicion, experienciaPrefab.transform.rotation);
            //Experiencia.GetComponent<Experiencia>().cantidadExperiencia = experienciaSoltar;
        }
        // Puedes agregar animaciones de muerte aquí si lo deseas
        // Por ejemplo: animator.SetTrigger("Muerte");

        // Destruir al enemigo
        Destroy(gameObject);
    }

    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            Petrificado = true;
            animator.SetBool("Petrificado", true);
            isFrozen = true;
            speed = 0;
            rb.velocity = Vector2.zero;
            /*if (spriteRenderer != null)
            {
                originalMaterial = spriteRenderer.material;
                spriteRenderer.material = grayscaleMaterial;
            }*/
            /*if (animator != null)
            {
                animator.enabled = false;
            }*/
            StartCoroutine(UnfreezeAfterTime(duration));
        }
    }
    IEnumerator UnfreezeAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        Petrificado = false;
        animator.SetBool("Petrificado", false);
        speed = originalSpeed;
        /*if (spriteRenderer != null)
        {
            spriteRenderer.material = originalMaterial;
        }*/
        /*if (animator != null)
        {
            animator.enabled = true;
        }*/
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}