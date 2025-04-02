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
    public float cooldownPegasoTime = 4f;
    private bool onCooldownPegaso = false;
    public Image CooldownFillPegaso;
    public TextMeshProUGUI CooldownTextPegaso;

    public float cooldownBloqueo = 3f;
    private bool onCooldownBloqueo = false;
    public float duracionBloqueo = 1f;
    private bool bloqueando = false;
    public Image CooldownFillBloqueo;
    public TextMeshProUGUI CooldownTextBloqueo;

    private bool enSuelo;
    private bool recibiendoDanio;
    public bool muerto;
    private bool atacando;
    private bool caminar;
    private bool salto;
    private bool ataquemedusa;
    private bool block;
    private bool pegaso;
    private bool atacandoFuerte;


    public float resistenciaMax = 100f;
    public float resistenciaActual;
    public float consumoCorrer = 10f;
    public float consumoRodar = 10f;
    public float regeneracion = 5f;
    public Image barraResistencia;

    public int experienciaActual = 0;
    public TextMeshProUGUI contadorExperiencia;
    //Recuperar Experiencia
    //public Vector2 ultimaPosicionMuerte;
    //public int experienciaPerdida = 0;

    public Transform puntoRespawn;
    public float tiempoRespawn = 2f;

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

        resistenciaActual = resistenciaMax;
        ActualizarBarraResistencia();

        ActualizarUIExperiencia();

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
        if(CooldownFillPegaso != null)
        {
            CooldownFillPegaso.fillAmount = 1;  
        }
        if(CooldownTextPegaso != null)
        {
            CooldownTextPegaso.text = "";
        }
    }

    void Update()
    {
        if (!muerto)
        {
            if (!recibiendoDanio)
            {
                if (!rodando && !atacando && !atacandoFuerte && !bloqueando)
                {
                    ProcesarMovimiento();
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);

                    if (Input.GetKeyDown(KeyCode.LeftShift) && enSuelo)
                    {
                        StartCoroutine(Rodar());
                    }

                    enSuelo = hit.collider != null;

                    //Salto
                    if (enSuelo && Input.GetKeyDown(KeyCode.Space))
                    {
                        rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
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
                    if (Input.GetKeyDown(KeyCode.LeftControl) && puedeCorrer && resistenciaActual > 0)
                    {
                        velocidad = velocidadExtra;
                        estaCorriendo = true;
                        resistenciaActual -= consumoCorrer * Time.deltaTime;
                        if (resistenciaActual <= 0)
                        {
                            puedeCorrer = false;
                            estaCorriendo = false;
                            velocidad = velocidadDeMovimientoBase;
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.LeftControl) || resistenciaActual <= 0)
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
                if (Input.GetKeyDown(KeyCode.F) && !atacando && enSuelo)
                {
                    Atacando(false);
                }

                //Ataque Fuerte
                if (Input.GetKeyDown(KeyCode.G) && !atacandoFuerte && enSuelo)
                {
                    Atacando(true);
                }

                if (Input.GetKeyDown(KeyCode.U) && !atacando && enSuelo && !isInvisible && !onCooldown)
                {
                    StartCoroutine(BecomeInvisible());
                }

                if (Input.GetKeyDown(KeyCode.I) && !onFreezeCooldown)
                {
                    FreezeEnemies();
                }
                if (onFreezeCooldown)
                {
                    StartCoroutine(FreezeCooldown());
                }

                if (Input.GetKeyDown(KeyCode.O) && pegasoHabilidad != null && !onCooldownPegaso)
                {
                    Pegaso();
                }

                if(Input.GetKeyDown(KeyCode.H)&& !onCooldownBloqueo)
                {
                    StartCoroutine(Bloquear());
                }
                if (!estaCorriendo && !rodando && resistenciaActual < resistenciaMax)
                {
                    resistenciaActual += regeneracion * Time.deltaTime;
                    if (resistenciaActual > resistenciaMax)
                    {
                        resistenciaActual = resistenciaMax;
                    }
                }
                ActualizarBarraResistencia();
            }
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("Caminar", caminar);
        animator.SetBool("Rodando", rodando);
        animator.SetBool("AtaqueMedusa", ataquemedusa);
        animator.SetBool("Block", block);
        animator.SetBool("atacado", recibiendoDanio);
        animator.SetBool("muelto", muerto);
        animator.SetBool("pegaso", pegaso);
        animator.SetBool("AtacandoFuerte", atacandoFuerte);
    }

    IEnumerator Rodar()
    {
        if (resistenciaActual < consumoRodar) yield break;
        resistenciaActual -= consumoRodar;

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
    void Pegaso()
    {
        Vector3 direccionCarga = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        pegasoHabilidad.ActivarCarga(transform.position, direccionCarga);
        animator.SetBool("pegaso", true);
        onCooldownPegaso = true;
        StartCoroutine(PegasoCooldown());
    }
    void DesactivarPegaso()
    {
        animator.SetBool("pegaso", false);
    }
    IEnumerator PegasoCooldown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < cooldownPegasoTime)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = cooldownPegasoTime - elapsedTime;
            if (CooldownFillPegaso != null)
            {
                CooldownFillPegaso.fillAmount = remainingTime / cooldownPegasoTime;
            }
            if (CooldownTextPegaso != null)
            {
                CooldownTextPegaso.text = Mathf.Ceil(remainingTime).ToString();
            }
            yield return null;
        }
        if (CooldownFillPegaso != null)
        {
            CooldownFillPegaso.fillAmount = 1;
        }
        if(CooldownTextPegaso != null)
        {
            CooldownTextPegaso.text = "";
        }
        onCooldownPegaso = false;
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
        animator.SetBool("recibiendoDanio", true);

        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            vida -= cantDanio;
            atacando = false;
            animator.SetBool("Atacando", false);
            if (vida <= 0)
            {
                muerto = true;
                animator.SetBool("muelto", true);
                StartCoroutine(Respawnear());
            }
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivarDanio());
            }
        }
    }
    IEnumerator Respawnear()
    {
        yield return new WaitForSeconds(tiempoRespawn);
        transform.position = puntoRespawn.position;
        vida = 5;
        muerto = false;
        recibiendoDanio = false;
        animator.SetBool("muelto", false);
        animator.SetBool("recibiendoDanio", false);
        rb.velocity = Vector2.zero;
    }
    IEnumerator DesactivarDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
        animator.SetBool("recibiendoDanio", false);

    }
    public void Atacando(bool ataqueFuerte)
    {
        if (ataqueFuerte)
        {
            atacandoFuerte = true;
            animator.SetTrigger("ataqueFuertePlayer");
            StartCoroutine(EsperarFinAtaqueFuerte());
        }
        else
        {
            atacando = true;
            animator.SetTrigger("ataqueplayer");
        }
    }
    public void DesactivarAtaque()
    {
        atacando = false;
        atacandoFuerte = false;
    }
    IEnumerator EsperarFinAtaqueFuerte()
    {
        // Espera un tiempo que dure la animación de ataque fuerte
        yield return new WaitForSeconds(0.433f);  // Ajusta el tiempo según la duración de la animación
        atacandoFuerte = false;  // Detiene el estado de ataque fuerte
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
        animator.SetBool("ataquemedusa", true);
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
    IEnumerator Bloquear()
    {
        bloqueando = true;
        invencible = true;
        animator.SetBool("block", true);

        yield return new WaitForSeconds(duracionBloqueo);

        bloqueando = false;
        invencible = false;
        StartCoroutine(BloqueoCooldown());
    }
    IEnumerator BloqueoCooldown()
    {
        onCooldownBloqueo = true;
        float elapsedTime = 0f;
        while (elapsedTime < cooldownBloqueo)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = cooldownBloqueo - elapsedTime;
            if (CooldownFillBloqueo != null)
            {
                CooldownFillBloqueo.fillAmount = remainingTime / cooldownBloqueo;
            }
            if (CooldownTextBloqueo != null)
            {
                CooldownTextBloqueo.text = Mathf.Ceil(remainingTime).ToString();
            }
            yield return null;
        }
        if (CooldownFillBloqueo != null) CooldownFillBloqueo.fillAmount = 1;
        if (CooldownTextBloqueo != null) CooldownTextBloqueo.text = "";
        onCooldownBloqueo = false;
    }

    void ActualizarBarraResistencia()
    {
        if (barraResistencia != null)
        {
            barraResistencia.fillAmount = resistenciaActual / resistenciaMax;
        }
    }
    void DesactivarMedusa()
    {
        animator.SetBool("ataquemedusa", false);
    }
    void DesactivarBlock()
    {
        animator.SetBool("block", false);
    }
    public void AgregarExperiencia(int cantidad)
    {
        experienciaActual += cantidad;
        ActualizarUIExperiencia();
    }
    void ActualizarUIExperiencia()
    {
        if(contadorExperiencia != null)
        {
            contadorExperiencia.text = "XP:" + experienciaActual;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EspadaSoldado"))
        {
            Debug.Log("atacado por enemigo");
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
        }
    }
}
