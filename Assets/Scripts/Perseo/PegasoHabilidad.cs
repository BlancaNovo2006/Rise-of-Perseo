using System.Collections;
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
    private Coroutine movimientoCoroutine;

    void Awake()
    {
        camara = Camera.main;
        pegasoCollider = GetComponent<Collider2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (player != null)
            perseoCollider = player.GetComponent<Collider2D>();

        gameObject.SetActive(false);
    }

    public void ActivarCarga(Vector3 posicionInicial, Vector3 direccionCarga)
    {
        Debug.Log("Pegaso Activado " + posicionInicial);

        posicionInicial.z -= 5;
        transform.position = posicionInicial;
        direccion = new Vector3(direccionCarga.x, 0, 0).normalized;

        if (pegasoCollider && perseoCollider)
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, true);

        gameObject.SetActive(true);
        movimientoCoroutine = StartCoroutine(MoverDuranteCarga());
    }

    private IEnumerator MoverDuranteCarga()
    {
        float tiempoTranscurrido = 0f;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.gravityScale = 0f;

        while (tiempoTranscurrido < duracion)
        {
            transform.position += direccion * velocidad * Time.deltaTime;

            if (FueraDePantalla())
                break;

            if (player)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 0);
            }

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        FinalizarCarga();
        yield break; // evita el error CS0161
    }

    private void FinalizarCarga()
    {
        if (movimientoCoroutine != null)
        {
            StopCoroutine(movimientoCoroutine);
            movimientoCoroutine = null;
        }

        if (pegasoCollider && perseoCollider)
            Physics2D.IgnoreCollision(pegasoCollider, perseoCollider, false);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.gravityScale = 1f;

        gameObject.SetActive(false);
    }

    private bool FueraDePantalla()
    {
        if (!camara) return false;

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
