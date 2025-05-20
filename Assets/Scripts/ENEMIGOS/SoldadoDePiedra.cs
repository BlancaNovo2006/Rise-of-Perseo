using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoldadoDePiedra : MonoBehaviour
{
    public AudioClip sonidoImpactoEspada;
    public AudioClip sonidoMuerteEnemigo;
    public AudioClip sonidoAtaqueSoldado;
    private AudioSource audioSource;

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

    protected bool isFrozen = false;
    protected float originalSpeed;
    protected SpriteRenderer spriteRenderer;

    protected Animator animator;
    public Collider2D espadaPiedraCollider;

    public Material grayscaleMaterial;
    private Material originalMaterial;

    protected bool Petrificado = false;
    void Start()
    {
        playervivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpeed = speed; DesactivarEspadaCollider();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sonidoAtaqueSoldado;
        audioSource.loop = true;  // Para que suene continuamente mientras camina
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


        animator.SetBool("caminando", EnMovimiento);
        animator.SetBool("Atacando", Atacando);

        if (!playervivo)
        {
            EnMovimiento = false;
            Atacando = false;
            movement = Vector2.zero;
        }
        if (EnMovimiento && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (!EnMovimiento && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

    }
    protected void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (!Atacando)
        {
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
                EnMovimiento = false;
                movement = Vector2.zero;
            }
        }
        else
        {
            Atacando = false;
            EnMovimiento = true;
            Movimiento();
            DesactivarEspadaCollider();
        }
    }
    public void ActivarEspadaCollider()
    {
        if (espadaPiedraCollider != null)
        {
            espadaPiedraCollider.enabled = true;
        }
    }
    public void DesactivarEspadaCollider()
    {
        if (espadaPiedraCollider != null)
        {
            espadaPiedraCollider.enabled = false;
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
            AudioManager.instance.ReporducirSonido(sonidoImpactoEspada);
            recibiendoDanio = true;
            animator.SetBool("recibiendoDanio", true);

            // Reducir las vidas del enemigo
            vidas -= cantDanio;

            Atacando = false;
            animator.SetBool("Atacando", false);
            //animator.Play("Idle", 0);
            

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
        AudioManager.instance.ReporducirSonido(sonidoMuerteEnemigo);
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