using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;

public class ControladorEnemigos : MonoBehaviour
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
    protected bool EnMovimiento;
    protected bool recibiendoDanio;
    protected bool Atacando;

    protected bool canseePlayer = true;

    protected bool isFrozen = false;
    protected float originalSpeed;
    protected SpriteRenderer spriteRenderer;

    protected Animator animator;

    public Material grayscaleMaterial;
    private Material originalMaterial;

    void Start()
    {
        playervivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpeed = speed;
        EnemyManager.Instance.RegistrarEnemigo(gameObject);
    }

    protected void Update()
    {
        Debug.Log("update base");
        if (player != null && playervivo && !muerto && !isFrozen)
        {
            
                Movimiento();
                if (transform.position == player.position)
                {
                    movement = new Vector2(0, 0);
                }
            
        }


        animator.SetBool("caminando", EnMovimiento);

        if (!playervivo)
        {
            EnMovimiento = false;
        }
    }
    protected void Movimiento()
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
        yield return new WaitForSeconds(0.2f);
        recibiendoDanio = false;
    }

    protected void Muerte()
    {
        muerto = true;
        EnemyManager.Instance.EliminarEnemigo(gameObject);
        
        Destroy(gameObject);
    }

    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            speed = 0;
            rb.velocity = Vector2.zero;
            if (spriteRenderer != null)
            {
                originalMaterial = spriteRenderer.material;
                spriteRenderer.material = grayscaleMaterial;
            }
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
        if (spriteRenderer != null)
        {
            spriteRenderer.material = originalMaterial;
        }
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