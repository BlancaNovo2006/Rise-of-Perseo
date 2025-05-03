using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sirena : MonoBehaviour
{
    public Transform player;
    public float detectionRadius;
    public float attackRadius;
    public float speed;
    public float fuerzaRebote;
    public int vidas;  // Vidas del enemigo

    public GameObject experienciaPrefab;
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

    public Collider2D EspadaSirena;
    
    protected Animator animator;
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
                    transform.localScale = new Vector3(-1, 1, 0);
                }
                if (direction.x > 0)
                {
                    transform.localScale = new Vector3(1, 1, 0);
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

    protected void ActivarAtaqueSirena()
    {
        if (!isFrozen)
        {
            EspadaSirena.enabled = true;
        }
    }
    protected void DesactivarAtaqueSirena()
    {
        if (!isFrozen)
        {
            EspadaSirena.enabled = false;
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
            //animator.SetBool("AtaqueCola", false);

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
        yield return new WaitForSeconds(0.6f);
        recibiendoDanio = false;
        animator.SetBool("recibiendoDanio", false);

        movement = Vector2.zero;
        Atacando = true;
    }

    protected void Muerte()
    {
        muerto = true;
        animator.SetBool("EstaMuerta", true);
        //if (experienciaPrefab != null)
        //{
        //Vector3 posicion = transform.position;
        //GameObject Experiencia = Instantiate(experienciaPrefab, posicion, experienciaPrefab.transform.rotation);
        //Experiencia.GetComponent<Experiencia>().cantidadExperiencia = experienciaSoltar;
        //}
        // Puedes agregar animaciones de muerte aquí si lo deseas
        // Por ejemplo: animator.SetTrigger("Muerte");

        StartCoroutine(EsperarMuerte());
    }
    IEnumerator EsperarMuerte()
    {
        // Espera el tiempo de duración de la animación de muerte anticipada
        yield return new WaitForSeconds(0.483f); // Ajusta este tiempo según la duración de la animación
        Destroy(gameObject); // Destruir el enemigo después de que la animación haya terminado
    }

    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            speed = 0;
            rb.velocity = Vector2.zero;
            spriteRenderer.color = Color.blue;
            if (animator != null)
            {
                animator.enabled = false;
            }
            StartCoroutine(UnfreezeAfterTime(duration));
        }
    }

    IEnumerator UnfreezeAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        isFrozen = false;
        speed = originalSpeed;
        spriteRenderer.color = Color.white;
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
