using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorEnemigos : MonoBehaviour
{
    public Transform player;
    public float detectionRadius;
    public float speed;
    public float fuerzaRebote;
    public int vidas = 3;  // Vidas del enemigo

    public GameObject experienciaPrefab;
    public int experienciaSoltar = 20;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool playervivo;
    private bool muerto;
    private bool EnMovimiento;
    private bool recibiendoDanio;
    private bool Atacando;

    private bool canseePlayer = true;

    private bool isFrozen = false;
    private float originalSpeed;
    private SpriteRenderer spriteRenderer;

    private Animator animator;

    void Start()
    {
        playervivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpeed = speed;
    }

    void Update()
    {
        if (player != null && playervivo && !muerto && !isFrozen)
        {
            MovimientoPersonaje playerScript = player.GetComponent<MovimientoPersonaje>();
            if (playerScript != null && playerScript.isInvisible)
            {
                canseePlayer = false;
            }
            else
            {
                canseePlayer = true;
                Movimiento();

                if (transform.position == player.position)
                {
                    movement = new Vector2(0, 0);
                }
            }
        }
        animator.SetBool("Atacando", Atacando);
        animator.SetBool("caminando", EnMovimiento);
    }
    private void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 0);
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 0);
            }

            movement = new Vector2(direction.x, 0);

            EnMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;

            EnMovimiento = false;
        }
    }

    void FixedUpdate()
    {
        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canseePlayer)
        {
            Vector2 direcciondanio = new Vector2(transform.position.x, 0);
            MovimientoPersonaje movimientoScript = collision.gameObject.GetComponent<MovimientoPersonaje>();

            movimientoScript.RecibeDanio(direcciondanio, 1);
            playervivo = !movimientoScript.muerto;

            //GameManager.Instance.PerderVida();

            if (!playervivo)
            {
                EnMovimiento = false;
            }
            Atacando = true;
        }
        else
        {
            Atacando = false;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
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
            // Reducir las vidas del enemigo
            vidas -= cantDanio;

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
        yield return new WaitForSeconds(0.2f);
        recibiendoDanio = false;
    }

    void Muerte()
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
        spriteRenderer.color = Color.red;
        if (animator != null)
        {
            animator.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}