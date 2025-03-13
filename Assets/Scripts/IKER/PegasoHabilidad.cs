using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegasoHabilidad : MonoBehaviour
{
    public Transform player;
    public float velocidad = 15f;
    public float duracion = 3f;

    private float tiempoRestante;
    private bool enCarga = false;

    private Vector3 direccion;

    private Camera camara;

    private Collider2D pegasoCollider;
    private Collider2D perseoCollider;
    private bool enMovimiento = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        camara = Camera.main;
        pegasoCollider = GetComponent<Collider2D>();
        perseoCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        if (player != null )
        {
            perseoCollider = player.GetComponent<Collider2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enMovimiento)
        {
            transform.position += direccion * velocidad * Time.deltaTime;

            if (FueraDePantalla())
            {
                FinalizarCarga();
            }
        }
    }
    public void ActivarCarga(Vector3 posicionInicial, Vector3 direccionCarga)
    {
        Debug.Log("Pegaso Activado" + posicionInicial);
        transform.position = posicionInicial;
        direccion = new Vector3(direccionCarga.x, 0, 0).normalized;
        if (pegasoCollider != null && perseoCollider != null)
        {
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, true);
        }
        gameObject.SetActive(true);
        enMovimiento = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
        StartCoroutine(DesactivarPegaso());
    }

    IEnumerator DesactivarPegaso()
    {
        yield return new WaitForSeconds(duracion);
        enMovimiento = false;
        if(pegasoCollider != null && perseoCollider != null)
        {
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, false);
        }
        gameObject.SetActive(false);
    }

    void FinalizarCarga()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D> ();
        if (rb != null) 
        {
            rb.gravityScale = 1f;
        }
        enMovimiento = false;
        if (pegasoCollider != null && perseoCollider != null)
        {
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, false);
        } 
        gameObject.SetActive(false);
    }
    bool FueraDePantalla()
    {
        if (camara == null) return false;

        Vector3 posicionEnPantalla = camara.WorldToViewportPoint(transform.position);
        return posicionEnPantalla.x < -0.1f || posicionEnPantalla.x > 1.1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            ControladorEnemigos enemigo = collision.GetComponent<ControladorEnemigos>();
            if (enemigo != null)
            {
                Vector2 direccionDanio = new Vector2(transform.position.x, 0);
                enemigo.RecibeDanio(direccionDanio, 1);
            }
        }
    }
}