using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MovimientoPersonaje : MonoBehaviour
{
    public int vida = 5;

    public float velocidad;
    public float fuerzaSalto = 10f;
    public float fuerzaRebote = 6f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;

    public float invisibilityDuration = 5f;
    public float invisibleAlpha = 0.3f;
    public bool isInvisible = false;
    private SpriteRenderer spriteRenderer;
    private int enemyLayer;
    public float cooldownTime = 3f;
    private bool onCooldown = false;
    public Image CooldownFill;
    public TextMeshProUGUI CooldownText;

    public float freezeRadius = 5f;
    public LayerMask Enemy;
    public float freezeDuration = 3f;
    public float cooldownFreezeTime = 5f;
    private bool onFreezeCooldown = false;
    public Image CooldownFreezeFill;
    public TextMeshProUGUI CooldownFreezeText;

    public PegasoHabilidad pegasoHabilidad;
    private Vector3 direccionCarga;
    public float cooldownGPegasoTime = 4f;
    private bool onCooldownPegaso = false;
    public Image CooldownFillPegaso;
    public TextMeshProUGUI CooldownTextPegaso;

    private bool enSuelo;
    private bool recibiendoDanio;
    public bool muerto;
    private bool atacando;
    private bool caminar;
    private bool Salto;

    public float velocidadDeMovimientoBase;
    public float velocidadExtra;
    public float tiempoSprint;
    private float tiempoActualSprint;
    private float tiempoSiguienteSprint;

    public float tiempoEntreSprints;
    private bool puedeCorrer = true;
    private bool estaCorriendo = false;

    public float tiempoRodar = 0.5f;
    public float velocidadRodar = 10f;
    private bool rodando = false;
    private bool invencible = false;

    private Rigidbody2D rb;
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tiempoActualSprint = tiempoSprint;
        pegasoHabilidad = FindObjectOfType<PegasoHabilidad>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyLayer = LayerMask.NameToLayer("Enemy");
        if (spriteRenderer == null)
        {
            Debug.LogError("El objeto no tiene un SpriteRenderer asignado");
        }
        if (CooldownFill != null)
        {
            CooldownFill.fillAmount = 1;
        }
        if (CooldownText != null)
        {
            CooldownText.text = "";
        }
        if (CooldownFreezeFill != null)
        {
            CooldownFreezeFill.fillAmount = 1;
        }
        if(CooldownFreezeText != null)
        {
            CooldownFreezeText.text = "";
        }
    }

    void Update()
    {
        if (!muerto)
        {
            if (!recibiendoDanio)
            {
                if (!rodando && !atacando)
                {
                    ProcesarMovimiento();
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
                    enSuelo = hit.collider != null;

                    if (Input.GetKeyDown(KeyCode.LeftShift) && enSuelo)
                    {
                        StartCoroutine(Rodar());
                    }

                    enSuelo = hit.collider != null;

                    //Salto
                    if (enSuelo && Input.GetKeyDown(KeyCode.Space))
                    {
                        rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                        animator.SetBool("Salto", true);
                    }
                    //Planear
                    if (!enSuelo && Input.GetKey(KeyCode.Space))
                    {
                        if (rb.velocity.y <= 0f)
                        {
                            rb.gravityScale = 0.25f;
                        }

                    }
                    else
                    {
                        rb.gravityScale = 1f;
                    }
                    //Correr
                    if (Input.GetKeyDown(KeyCode.B) && puedeCorrer)
                    {
                        velocidad = velocidadExtra;
                        estaCorriendo = true;
                    }
                    if (Input.GetKeyUp(KeyCode.B))
                    {
                        velocidad = velocidadDeMovimientoBase;
                        estaCorriendo = false;
                    }
                    if (Mathf.Abs(rb.velocity.x) >= 0.1f && estaCorriendo)
                    {
                        if (tiempoActualSprint > 0)
                        {
                            tiempoActualSprint -= Time.deltaTime;
                        }
                        else
                        {
                            velocidad = velocidadDeMovimientoBase;
                            estaCorriendo = false;
                            puedeCorrer = false;
                            tiempoSiguienteSprint = Time.time + tiempoEntreSprints;
                        }
                    }
                    if (!estaCorriendo && tiempoActualSprint <= tiempoSprint && Time.time >= tiempoSiguienteSprint)
                    {
                        tiempoActualSprint += Time.deltaTime;
                        if (tiempoActualSprint >= tiempoSprint)
                        {
                            puedeCorrer = true;
                        }
                    }
                }
                //Atacar
                if (Input.GetKeyDown(KeyCode.Z) && !atacando && enSuelo)
                {
                    Atacando();
                }

                if (Input.GetKeyDown(KeyCode.X) && !atacando && enSuelo && !isInvisible && !onCooldown)
                {
                    StartCoroutine(BecomeInvisible());
                }

                if (Input.GetKeyDown(KeyCode.C) && !onFreezeCooldown)
                {
                    FreezeEnemies();
                }
                if (onFreezeCooldown)
                {
                    StartCoroutine(FreezeCooldown());
                }
                if (Input.GetKeyDown(KeyCode.V) && pegasoHabilidad != null && !onCooldownPegaso)
                {
                    Vector3 direccionCarga = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
                    pegasoHabilidad.ActivarCarga(transform.position, direccionCarga);
                }
            }
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("Caminar", caminar);
        animator.SetBool("Rodando", rodando);
        animator.SetBool("Salto", Salto);
    }

    IEnumerator Rodar()
    {
        rodando = true;
        invencible = true;
        animator.SetTrigger("Rodar");

        // Ignorar colisión entre Jugador y Enemigos
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        float direccion = transform.localScale.x; // 1 si mira derecha, -1 si mira izquierda
        rb.velocity = new Vector2(velocidadRodar * direccion, rb.velocity.y);

        yield return new WaitForSeconds(tiempoRodar);

        rb.velocity = Vector2.zero;  // Detiene el deslizamiento

        // Restaurar colisión entre Jugador y Enemigos
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        invencible = false;
        rodando = false;
    }
    void ProcesarMovimiento()
    {
        //Logica de movimiento
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(horizontal, 0) * velocidad * Time.deltaTime;

        caminar = horizontal != 0;

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
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (invencible) return; // Si es invencible, no recibe daño

        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            if (vida <= 0)
            {
                muerto = true;
            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivarDanio());
            }
        }
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
    }
    public void Atacando()
    {
        atacando = true;
    }
    public void DesactivarAtaque()
    {
        atacando = false;
    }
    IEnumerator BecomeInvisible()
    {
        SetCooldownOpacity(0.3f);
        if (CooldownFill != null)
        {
            CooldownFill.fillAmount = 1;
        }

        isInvisible = true;
        onCooldown = true;
        SetOpacity(invisibleAlpha);
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, true);
        yield return new WaitForSeconds(invisibilityDuration);
        SetOpacity(1f);
        Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, false);
        isInvisible = false;
        StartCoroutine(StartCooldown());
    }
    IEnumerator StartCooldown()
    {
        float elapsedTime = 0f;
        
        onCooldown = true;
        while (elapsedTime < cooldownTime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = cooldownTime - elapsedTime;

            //Actualizar la UI
            if (CooldownFill != null)
            {
                CooldownFill.fillAmount = remainingTime / cooldownTime; //Relleno radial
            }

            if (CooldownText != null)
            {
                CooldownText.text = Mathf.Ceil(remainingTime).ToString(); //Cuenta regresiva
            }
            yield return null;
        }

        //Reiniciar cooldown
        if (CooldownFill != null)
        {
            CooldownFill.fillAmount = 1;
        }
        if (CooldownText != null)
        {
            CooldownText.text = "";
        }

        SetCooldownOpacity(1f);
        onCooldown = false;
    }
    void SetOpacity(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }
    void SetCooldownOpacity(float alpha)
    {
        if (CooldownFill != null)
        {
            Color color = CooldownFill.color;
            color.a = alpha;
            CooldownFill.color = color;
        }
    }
    void FreezeEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, freezeRadius, Enemy);
        foreach (Collider2D Enemy in enemies)
        {
            ControladorEnemigos ControladorEnemigos = Enemy.GetComponent<ControladorEnemigos>();
            if (ControladorEnemigos != null)
            {
                ControladorEnemigos.Freeze(freezeDuration);
            }
        }
        onFreezeCooldown = true;
        if(CooldownFreezeFill != null)
        {
            CooldownFreezeFill.fillAmount = 1;
        }
        if (CooldownFreezeText != null)
        {
            CooldownFreezeText.text = Mathf.Ceil(cooldownFreezeTime).ToString();
        }
    }
    IEnumerator FreezeCooldown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < cooldownFreezeTime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = cooldownFreezeTime - elapsedTime;
            if(CooldownFreezeFill != null)
            {
                CooldownFreezeFill.fillAmount = remainingTime / cooldownFreezeTime;
            }
            if (CooldownFreezeText != null)
            {
                CooldownFreezeText.text = Mathf.Ceil(remainingTime).ToString();
            }
            yield return null;
        }
        if (CooldownFreezeFill != null)
        {
            CooldownFreezeFill.fillAmount = 1;
        }
        if(CooldownFreezeText != null)
        {
            CooldownFreezeText.text = "";
        }
        onFreezeCooldown = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

}
