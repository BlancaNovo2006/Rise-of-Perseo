using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PegasoHabilidad : MonoBehaviour
{
    public Transform player;
    public float velocidad = 15f;
    public float duracion = 3f;

    private Vector3 direccion;
    private Camera camara;
    private Collider2D pegasoCollider;
    private Collider2D perseoCollider;
    private bool enMovimiento = false;

    void Start()
    {
        // 👇 Esto se ejecutará solo si el objeto está ACTIVADO en la jerarquía.
        camara = Camera.main;
        pegasoCollider = GetComponent<Collider2D>();

        if (player != null)
        {
            perseoCollider = player.GetComponent<Collider2D>();
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                perseoCollider = playerObj.GetComponent<Collider2D>();
        }

        // 🔥 Aquí se desactiva automáticamente una vez se inicializa
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (enMovimiento)
        {
            transform.position += direccion * velocidad * Time.deltaTime;

            if (FueraDePantalla())
            {
                FinalizarCarga();
            }

            Vector2 direction = (player.position - transform.position).normalized;
            transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 0);
        }
    }

    public void ActivarCarga(Vector3 posicionInicial, Vector3 direccionCarga)
    {
        Debug.Log("Pegaso Activado " + posicionInicial);
        posicionInicial.z -= 5;
        transform.position = posicionInicial;
        direccion = new Vector3(direccionCarga.x, 0, 0).normalized;

        if (pegasoCollider != null && perseoCollider != null)
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, true);

        gameObject.SetActive(true);
        enMovimiento = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.gravityScale = 0f;

        StartCoroutine(DesactivarPegaso());
    }

    IEnumerator DesactivarPegaso()
    {
        yield return new WaitForSeconds(duracion);
        enMovimiento = false;

        if (pegasoCollider != null && perseoCollider != null)
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, false);

        gameObject.SetActive(false);
    }

    void FinalizarCarga()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.gravityScale = 1f;

        enMovimiento = false;

        if (pegasoCollider != null && perseoCollider != null)
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, false);

        gameObject.SetActive(false);
    }

    bool FueraDePantalla()
    {
        if (camara == null) return false;
        Vector3 pos = camara.WorldToViewportPoint(transform.position);
        return pos.x < -0.1f || pos.x > 1.1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
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