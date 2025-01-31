using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Personaje : MonoBehaviour
{
    public float speed = 10;
    public int vida = 3;

    public float fuerzasalto = 10;
    public float fuerzarebote = 10;
    public float longitudRaycast = 0.1f;
    public LayerMask capasuelo;
    public float velocidadDash;
    public float tiempoDash;
    private float gravedadInicial;

    private Rigidbody2D rb;
    private bool enSuelo;
    private bool recibiendodanio;
    private bool atacar;
    public bool muerto;
    private bool PuedeDash = true;
    private bool PuedeMover = true;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravedadInicial = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // = es para igualar/asignar un valor
        if (!muerto)
        {
            if (!atacar)
            {
                if (PuedeMover)
                {
                    Movimiento();

                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capasuelo);
                    enSuelo = hit.collider != null;

                    if (enSuelo && Input.GetKeyDown(KeyCode.Space))
                    {
                        rb.AddForce(new Vector2(0f, fuerzasalto), ForceMode2D.Impulse);
                    }

                    if (Input.GetKeyDown(KeyCode.B) && PuedeDash)
                    {
                        StartCoroutine(Dash());
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Z) && !atacar && enSuelo)
            {
                atacando();
            }
        }
        Animaciones();
    }

    public void Animaciones()
    {
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("atacando", atacar);
        animator.SetBool("recibedanio", recibiendodanio);
        animator.SetBool("muerto", muerto);
    }

    public void RecibeDanio(Vector2 direccion, int cantdanio)
    {
        if (!recibiendodanio)
        {
            recibiendodanio = true;
            vida -= cantdanio;
            if (vida <= 0)
            {
                muerto = true;            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzarebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivarDanio());
            }
        }
    }

    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendodanio = false;
        rb.velocity = Vector2.zero;
    }

    public void Movimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(horizontal, 0) * speed * Time.deltaTime;

        animator.SetFloat("movement", horizontal * speed);

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

    public void atacando()
    {
        atacar = true;
    }

    public void DesactivarAtaque()
    {
        atacar = false;
    }

    private IEnumerator Dash()
    {
        PuedeMover = false;
        PuedeDash = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(velocidadDash * transform.localScale.x, 0);
        animator.SetTrigger("dash");

        yield return new WaitForSeconds(tiempoDash);

        PuedeMover = true;
        PuedeDash = true;
        rb.gravityScale = gravedadInicial;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
