using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CangrejoColosal : MonoBehaviour
{
    public AudioClip sonidoImpactoEspada;
    public AudioClip sonidoMuerteEnemigo;
    public AudioClip sonidoCangrejo;
    public float speed;
    public Transform controladorSuelo;
    public float distancia;
    private bool moviendoDerecha;
    private Rigidbody2D rb;


    protected bool Petrificado = false;

    protected bool AtaquePinzas;
    protected bool AtaquePinchos;
    public Collider2D ZonaVertical;
    public Collider2D ZonaHorizontal;
    public Collider2D Pinchos;
    public Collider2D Pinzas;
    protected bool estaAtacando;

    public float fuerzaRebote;
    public int vidas;  // Vidas del enemigo
    public GameObject experienciaPrefab;
    public int experienciaSoltar = 20;
    protected bool playervivo;
    protected bool muerto;
    protected bool recibiendoDanio;
    protected bool isFrozen = false;
    protected float originalSpeed;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    public Material grayscaleMaterial;
    private Material originalMaterial;
    private bool direccionAntesDeCongelarse;
    private Vector3 escalaOriginal;


    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playervivo = true;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSpeed = speed;
        escalaOriginal = transform.localScale;
    }

    protected void FixedUpdate()
    {
        if (!muerto && !isFrozen && !estaAtacando)
        {
            RaycastHit2D informacionSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distancia);
            Vector2 direccion = moviendoDerecha ? Vector2.right : Vector2.left;

            rb.velocity = new Vector2(speed, rb.velocity.y);

            if (!informacionSuelo)
            {
                Girar();
            }
        }
        else if (isFrozen)
        {
            rb.velocity = Vector2.zero;
        }
    }


    protected void Girar()
    {
        moviendoDerecha = !moviendoDerecha;

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;

        speed *= -1;
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
        yield return new WaitForSeconds(0.2f);
        recibiendoDanio = false;
        animator.SetBool("recibiendoDanio", false);

    }
    protected void Muerte()
    {
        AudioManager.instance.ReporducirSonido(sonidoMuerteEnemigo);
        rb.velocity = Vector2.zero;
        muerto = true;
        animator.SetBool("EstaMuerto", true);
        //if (experienciaPrefab != null)
        //{
        //Vector3 posicion = transform.position;
        //GameObject Experiencia = Instantiate(experienciaPrefab, posicion, experienciaPrefab.transform.rotation);
        //Experiencia.GetComponent<Experiencia>().cantidadExperiencia = experienciaSoltar;
        //}
        // Puedes agregar animaciones de muerte aqu� si lo deseas
        // Por ejemplo: animator.SetTrigger("Muerte");

        StartCoroutine(EsperarMuerte());
    }
    IEnumerator EsperarMuerte()
    {
        // Espera el tiempo de duraci�n de la animaci�n de muerte anticipada
        yield return new WaitForSeconds(0.683f); // Ajusta este tiempo seg�n la duraci�n de la animaci�n
        Destroy(gameObject); // Destruir el enemigo despu�s de que la animaci�n haya terminado
    }
    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            Petrificado = true;
            animator.SetBool("Petrificado", true);
            direccionAntesDeCongelarse = moviendoDerecha;
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
        moviendoDerecha = direccionAntesDeCongelarse;
        isFrozen = false;
        Petrificado = false;
        animator.SetBool("Petrificado", false);

        // Restaurar velocidad seg�n direcci�n original
        speed = Mathf.Abs(originalSpeed) * (moviendoDerecha ? 1 : -1);

        // Restaurar la escala del sprite seg�n la direcci�n
        Vector3 escala = escalaOriginal;
        escala.x = Mathf.Abs(escala.x) * (moviendoDerecha ? 1 : -1);
        transform.localScale = escala;

        /*if (spriteRenderer != null)
        {
            spriteRenderer.material = originalMaterial;
        }*/
        /*if (animator != null)
        {
            animator.enabled = true;
        }*/
    }
    protected void ActivarAtaquePinchos()
    {
        if (!isFrozen)
        {
            Pinchos.enabled = true;
        }
    }
    protected void DesactivarAtaquePinchos()
    {
        if (!isFrozen)
        {
            Pinchos.enabled = false;
        }
    }
    public void ActivarZonaVertical()
    {
        Debug.Log("ZonaVertical activada - el jugador est� dentro.");
        estaAtacando = true;
        AtaquePinchos = true;
        animator.SetBool("AtaquePinchos", true);
    }

    public void DesactivarZonaVertical()
    {
        Debug.Log("ZonaVertical desactivada - el jugador sali�.");
        estaAtacando = false;
        AtaquePinchos = false;
        animator.SetBool("AtaquePinchos", false);
    }
    protected void ActivarAtaquePinzas()
    {
        if (!isFrozen)
        {
            AudioManager.instance.ReporducirSonido(sonidoCangrejo);
            Pinzas.enabled = true;
        }
    }
    protected void DesactivarAtaquePinzas()
    {
        if (!isFrozen)
        {
            Pinzas.enabled = false;
        }
    }
    public void ActivarZonaHorizontal()
    {
        Debug.Log("ZonaHorizontal activada - el jugador est� dentro.");
        estaAtacando = true;
        AtaquePinzas = true;
        animator.SetBool("AtaquePinzas", true);
    }

    public void DesactivarZonaHorizontal()
    {
        Debug.Log("ZonaHorizontal desactivada - el jugador sali�.");
        estaAtacando = false;
        AtaquePinzas = false;
        animator.SetBool("AtaquePinzas", false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(controladorSuelo.transform.position, controladorSuelo.transform.position + Vector3.down * distancia);
    }

    
}
