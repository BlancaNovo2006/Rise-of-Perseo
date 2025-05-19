using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Build;
using UnityEngine;

public class Medusa : MonoBehaviour
{
    public AudioClip sonidoImpactoEspada;
    public AudioClip sonidoMuerteEnemigo;
    public Transform player;
    public Transform posicionInicial;
    public float detectionRadius;
    public float attackRadius;
    public float shootRadius;
    public float speed;
    public float fuerzaRebote;
    public int vidas = 5;  // Vidas del enemigo

    public GameObject experienciaPrefab;
    public Collider2D colaMedusaCollider;
    public int experienciaSoltar = 20;
    public GameObject SerpientePrefab;
    public float velocidadSerpiente;
    private MovimientoPersonaje movimientoPersonaje;

    protected Rigidbody2D rb;
    protected Vector2 movement;
    protected bool playervivo;
    public bool muerto;
    protected bool EnMovimiento;
    protected bool recibiendoDanio;
    protected bool AtaqueCola;
    protected bool AtaqueGrito;
   

    public float tiempoEntreLanzamientos = 2f; // Tiempo en segundos entre lanzamientos de serpientes
    private bool puedeLanzarSerpiente = true; // Flag para controlar el lanzamiento

    protected bool canseePlayer = true;

    protected bool isFrozen = false;
    protected float originalSpeed;
    protected SpriteRenderer spriteRenderer;

    protected Animator animator;
    private bool esperandoMuertePlayer = false;

    //protected bool muerto = false;
    public static bool EstaMuerto = false;
    //public bool EstaMuerto => muerto;

    public Material grayscaleMaterial;
    private Material originalMaterial;

    void Start()
    {
        transform.position = posicionInicial.position;
        playervivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpeed = speed;

        if (player != null)
        {
            movimientoPersonaje = player.GetComponent<MovimientoPersonaje>();
        }
    }

    protected void Update()
    {
        if (player != null && playervivo && !muerto && !isFrozen)
        {
            
                Movimiento();
                AtaqueColaMedusa();
                AtaqueGritoMedusa();
                if (transform.position == player.position)
                {
                    movement = new Vector2(0, 0);
                }
            
        }
        animator.SetBool("EnMovimiento", EnMovimiento);
        animator.SetBool("AtaqueCola", AtaqueCola);
        animator.SetBool("AtaqueGrito", AtaqueGrito);
        if (movimientoPersonaje != null &&  movimientoPersonaje.vida <= 0)
        {
            playervivo = false;
            if (!esperandoMuertePlayer)
            {
                esperandoMuertePlayer = true;
                StartCoroutine(VolverAPosicionInicial());
            }
        }
        else
        {
            playervivo = true;
            esperandoMuertePlayer = false;
        }
        
    }
    IEnumerator VolverAPosicionInicial()
    {
        yield return new WaitForSeconds(2f);
        transform.position = posicionInicial.position;
        vidas = 5;
        EnMovimiento = false;
    }
    protected void Movimiento()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 0);
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 0);
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
    protected void AtaqueGritoMedusa()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < shootRadius)
        {
            if (!AtaqueGrito && !recibiendoDanio && !muerto)
            {
                //AtaqueGrito = true;
                //animator.SetBool("AtaqueGrito", AtaqueGrito);
                Invoke("DesactivarAtaqueGrito", 0.7f);
                LanzarSerpientes();
            }
        }
        else if (distanceToPlayer > shootRadius)
        {
            AtaqueGrito = false;
            animator.SetBool("AtaqueGrito", false);
        }
    }
    void DesactivarAtaqueGrito()
    {
        AtaqueGrito = false;
        animator.SetBool("AtaqueGrito", false);
    }

    protected void LanzarSerpientes()
    {
        if (player == null || !puedeLanzarSerpiente) return;

        if (SerpientePrefab == null)
        {
            Debug.LogError("SerpientePrefab no asignado.");
            return;
        }

        Debug.Log("ataquelanzado");

        Vector2 direccion = (player.position - transform.position).normalized;
        Vector2 spawnPos = (Vector2)transform.position + direccion * 0.5f;

        GameObject Serpiente = Instantiate(SerpientePrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rbSerpiente = Serpiente.GetComponent<Rigidbody2D>();
        if (rbSerpiente != null)
        {
            rbSerpiente.velocity = direccion * velocidadSerpiente;
        }

        // Rotar la serpiente solo en Z
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        Serpiente.transform.rotation = Quaternion.Euler(0, 0, angulo);

        // Voltear en X si va hacia la izquierda (usando escala)
        float direccionX = direccion.x >= -1 ? 1f : -1f;
        Vector3 escalaOriginal = Serpiente.transform.localScale;
        Serpiente.transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x) * direccionX, escalaOriginal.y, escalaOriginal.z);
        AtaqueGrito = true;
        animator.SetBool("AtaqueGrito", true);

        StartCoroutine(TiempoDeEspera());
    }
    private IEnumerator TiempoDeEspera()
    {
        puedeLanzarSerpiente = false; // Desactivar la capacidad de lanzar serpientes
        yield return new WaitForSeconds(tiempoEntreLanzamientos); // Esperar el tiempo especificado
        puedeLanzarSerpiente = true; // Permitir el lanzamiento nuevamente
        AtaqueGrito = false;
        animator.SetBool("AtaqueGrito", false);
    }
    protected void AtaqueColaMedusa()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRadius)
        {
            if (!AtaqueCola)
            {
                AtaqueCola = true;
                EnMovimiento = false;
                movement = Vector2.zero;
            }
        }
        else
        {
            AtaqueCola = false;
            EnMovimiento = true;
            Movimiento();
            DesactivarColaCollider();
        }
    }
    public void ActivarColaCollider()
    {
        if (colaMedusaCollider != null)
        {
            colaMedusaCollider.enabled = true;
        }
    }
    public void DesactivarColaCollider()
    {
        if (colaMedusaCollider != null)
        {
            colaMedusaCollider.enabled = false;
        }
    }
    void FixedUpdate()
    {
        if (!recibiendoDanio && rb != null)
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
            AudioManager.instance.ReporducirSonido(sonidoImpactoEspada);
            recibiendoDanio = true;
            animator.SetBool("recibiendoDanio", true);

            // Reducir las vidas del enemigo
            vidas -= cantDanio;
            EnMovimiento = false;
            AtaqueCola = false;
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
        yield return new WaitForSeconds(1f);
        recibiendoDanio = false;
        animator.SetBool("recibiendoDanio", false);

        movement = Vector2.zero;
        EnMovimiento = true;
        AtaqueCola = true;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Muerte()
    {
        AudioManager.instance.ReporducirSonido(sonidoMuerteEnemigo);
        EstaMuerto = true;
        animator.SetBool("EstaMuerta", true );
        Debug.Log("Medusa ha muerto.");
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
    private IEnumerator EsperarMuerte()
    {
        // Espera el tiempo de duración de la animación de muerte anticipada
        yield return new WaitForSeconds(1f); // Ajusta este tiempo según la duración de la animación
        Destroy(gameObject); // Destruir el enemigo después de que la animación haya terminado
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, shootRadius);
    }
}
