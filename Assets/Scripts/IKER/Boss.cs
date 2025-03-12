using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator animator;
    public float detectionRadius = 5.0f;
    public Rigidbody2D rb;
    public Transform player;
    private bool mirandoDerecha = true;
    public int vida = 3;
    private bool daniorecibido;
    private bool muerto;
    public float fuerzarebote = 10;

    [Header("Vida")]
    [SerializeField] private float maximoVida;
    //[SerializeField] private BarraDeVida barraDeVida;

    [Header("Ataque")]
    [SerializeField] private Transform Ataque1;
    [SerializeField] private Transform Ataque2;
    [SerializeField] private float radioAtaque;
    [SerializeField] private int cantdanio;

    private bool canseePlayer = true;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //barraDeVida.InicializarBarraDeVida(vida);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanciaplayer = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("distanciaplayer", distanciaplayer);
        MirarJugador();
    }

    public void RecibirDanio(int danio)
    {
        daniorecibido = true;
        vida -= danio;

        //barraDeVida.CambiarVidaActual(vida);

        if (vida <= 0)
        {
            animator.SetTrigger("Muerte");
        }
    }
    private void Muerte()
    {
        Destroy(gameObject);
    }

    public void MirarJugador()
    {
        if ((player.position.x > transform.position.x && !mirandoDerecha) || (player.position.x < transform.position.x && mirandoDerecha))
        {
            mirandoDerecha = !mirandoDerecha;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
    }

    public void AtaqueUno()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(Ataque1.position, radioAtaque);

        foreach (Collider2D colision in objetos)
        {
            if (colision.CompareTag("Player"))
            {
                Vector2 direccion = (colision.transform.position - transform.position).normalized;
                colision.GetComponent<MovimientoPersonaje>().RecibeDanio(direccion, cantdanio);

            }
        }
    }

    public void AtaqueDos()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(Ataque2.position, radioAtaque);

        foreach (Collider2D colision in objetos)
        {
            if (colision.CompareTag("Player"))
            {
                Vector2 direccion = (colision.transform.position - transform.position).normalized;
                colision.GetComponent<MovimientoPersonaje>().RecibeDanio(direccion, cantdanio);

            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantdanio)
    {
        if (!daniorecibido)
        {
            vida -= cantdanio;
            daniorecibido = true;
            if (vida <= 0)
            {
                muerto = true;
                animator.SetTrigger("Muerte");

            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzarebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivarDanio());
            }
        }
        else
        {
            daniorecibido = false;
        }

    }

    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.4f);
        daniorecibido = false;
        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Ataque1.position, radioAtaque);

        //Gizmos.color += Color.yellow;
        //Gizmos.DrawWireSphere(Ataque2.position, radioAtaque);
    }

}

