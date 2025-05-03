using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MovimientoPersonaje : MonoBehaviour
{
    public Transform CheetTp1;
    public Transform CheetTp2;
    public Transform CheetTpSpawn;
    private Vector2 posicionTp1;
    private Vector2 posicionTp2;
    private Vector2 posicionTpSpawn;

    public int vida = 5;
    public int vialRegenerativo = 5;

    public float velocidad;
    public float fuerzaSalto = 10f;
    public float fuerzaRebote = 6f;
    public float longitudRaycast = 0.3f;
    public LayerMask capaSuelo;

   
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

    
    public Image CooldownFillBloqueo;
    public TextMeshProUGUI CooldownTextBloqueo;

    private bool enSuelo;
    private bool recibiendoDanio;
    public bool muerto;
    private bool atacando;
    private bool caminar;
    private bool salto;
    private bool ataquemedusa;
    private bool pegaso;
    private bool planeando;
    private bool enDash;


    

    public int experienciaActual = 0;
    public TextMeshProUGUI contadorExperiencia;
    //Recuperar Experiencia
    //public Vector2 ultimaPosicionMuerte;
    //public int experienciaPerdida = 0;

    public Transform puntoRespawn;
    public float tiempoRespawn = 2f;

    public float tiempoRodar = 0.5f;
    public float velocidadRodar = 10f;
    private bool rodando = false;
    private bool invencible = false;

    private Rigidbody2D rb;
    public Animator animator;

    private float coyoteTimeCounter;
    public float coyoteTimeDuration = 0.2f;

    private Vector2 ultimoPuntoRespawn;

    private int habilidadActual = 0;
    private List<Habilidad> habilidades;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pegasoHabilidad = FindObjectOfType<PegasoHabilidad>();
        ultimoPuntoRespawn = puntoRespawn.position;
        posicionTp1 = CheetTp1.position;
        posicionTp2 = CheetTp2.position;
        posicionTpSpawn = CheetTpSpawn.position;


        habilidades = new List<Habilidad>
        {
            new Habilidad(FreezeEnemies, PuedeUsarFreezeEnemies),
            new Habilidad(Pegaso, PuedeUsarPegaso)
        };

        ActualizarUIVidas();

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
                if (!rodando && !atacando)
                {
                    ProcesarMovimiento();
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);

                    if (Input.GetKeyDown(KeyCode.LeftShift) && enSuelo)
                    {
                        StartCoroutine(Rodar());
                    }

                    enSuelo = hit.collider != null;

                    //CoyoteTime
                    if (enSuelo)
                    {
                        coyoteTimeCounter = coyoteTimeDuration;
                    }
                    else
                    {
                        coyoteTimeCounter -= Time.deltaTime;
                    }

                    //Salto
                    if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
                        coyoteTimeCounter = 0f; // Evita que se pueda saltar varias veces en el aire
                    }
                    //Planear
                    if (!enSuelo && Input.GetKey(KeyCode.Space))
                    {
                        if (rb.velocity.y <= 0f)
                        {
                            rb.gravityScale = 0.25f;
                        }
                        planeando = true;
                    }
                    else
                    {
                        rb.gravityScale = 1f;
                        planeando = false;
                    }
                    

                    //Regenerar Vida
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        if (vida < 5)
                        {
                            if (vialRegenerativo > 0)
                            {
                                StartCoroutine(RegenerarVida());
                                ActualizarUIVidas();

                            }
                        }
                    }
                    IEnumerator RegenerarVida()
                    {
                        // Cambiar el color a rojo
                        spriteRenderer.color = Color.red;

                        // Sumar 1 a la vida
                        vida += 1;
                        vialRegenerativo -= 1;
                        if (vialRegenerativo < 0)
                        {
                            vialRegenerativo = 0;
                        }

                        // Esperar 1 segundo
                        yield return new WaitForSeconds(0.3f);

                        // Restaurar el color a blanco
                        spriteRenderer.color = Color.white;
                    }

                    //Chetos
                    if (Input.GetKeyDown(KeyCode.F1))
                    {
                        transform.position = posicionTp1;
                    }
                    if (Input.GetKeyDown(KeyCode.F2))
                    {
                        transform.position = posicionTp2;
                    }
                    if (Input.GetKeyDown(KeyCode.F3))
                    {
                        transform.position = posicionTpSpawn;
                    }
                    if (Input.GetKeyDown(KeyCode.F4))
                    {
                        invencible = !invencible;
                    }
                }
                //Atacar
                if (Input.GetKeyDown(KeyCode.F) && !atacando && enSuelo)
                {
                    Atacando();
                }



                CambiarHabilidad();
                UsarHabilidad();

                //Cabeza Medusa
                /*if (Input.GetKeyDown(KeyCode.I) && !onFreezeCooldown)
                {
                    FreezeEnemies();
                }
                

                //Pegaso
                if (Input.GetKeyDown(KeyCode.O) && pegasoHabilidad != null && !onCooldownPegaso)
                {
                    Pegaso();
                }*/

                
               
            }
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("Caminar", caminar);
        animator.SetBool("Rodando", rodando);
        animator.SetBool("AtaqueMedusa", ataquemedusa);
        animator.SetBool("atacado", recibiendoDanio);
        animator.SetBool("muelto", muerto);
        animator.SetBool("pegaso", pegaso);
        animator.SetBool("planeando", planeando);
        animator.SetBool("endash", enDash);
    }
    protected void CambiarHabilidad()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            habilidadActual = (habilidadActual + 1) % habilidades.Count;
        }
        else if (scroll < 0f)
        {
            habilidadActual = (habilidadActual - 1 + habilidades.Count) % habilidades.Count;
        }
    }
    
    protected void UsarHabilidad()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (habilidades[habilidadActual].PuedeUsar())
            {
                habilidades[habilidadActual].Ejecutar();
            }
            else
            {
                Debug.Log("No puedes usar esta habilidad ahora.");
            }
        }
    }

    // ==== CONDICIONES DE USO ====

    bool PuedeUsarFreezeEnemies() => !onFreezeCooldown;
    bool PuedeUsarPegaso() => pegasoHabilidad != null && !onCooldownPegaso;

    // ==== ESTRUCTURA DE HABILIDAD ====

    private class Habilidad
    {
        public Action Ejecutar;
        public Func<bool> PuedeUsar;

        public Habilidad(Action ejecutar, Func<bool> puedeUsar)
        {
            Ejecutar = ejecutar;
            PuedeUsar = puedeUsar;
        }
    }
    IEnumerator Rodar()
    {
        

        rodando = true;
        enDash = true;
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
        enDash = false;
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
        transform.position = ultimoPuntoRespawn;
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
    public void Atacando()
    {
        atacando = true;
        animator.SetTrigger("ataqueplayer");
    }
    public void DesactivarAtaque()
    {
        atacando = false;
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
            GargolaDePiedra GargolaDePiedra = Enemy.GetComponent<GargolaDePiedra>();
            if (GargolaDePiedra != null)
            {
                GargolaDePiedra.Freeze(freezeDuration);
            }
            SoldadoDePiedra SoldadoDePiedra = Enemy.GetComponent<SoldadoDePiedra>();
            if (SoldadoDePiedra != null)
            {
                SoldadoDePiedra.Freeze(freezeDuration);
            }
            Medusa Medusa = Enemy.GetComponent<Medusa>();
            if (Medusa != null)
            {
                Medusa.Freeze(freezeDuration);
            }
            CangrejoColosal CangrejoColosal = Enemy.GetComponent<CangrejoColosal>();
            if (CangrejoColosal != null)
            {
                CangrejoColosal.Freeze(freezeDuration);
            }
            Sirena Sirena = Enemy.GetComponent<Sirena>();
            if (Sirena != null)
            {
                Sirena.Freeze(freezeDuration);
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
        StartCoroutine(FreezeCooldown());
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

    void ActualizarUIVidas()
    {
        if(CooldownTextBloqueo != null)
        {
            CooldownTextBloqueo.text = "" + vialRegenerativo.ToString();
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

        if (collision.CompareTag("Proyectil"))
        {
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);
            
        }

        if (collision.CompareTag("ColaMedusa"))
        {
            Vector2 direcciondanio = new Vector2(collision.gameObject.transform.position.x, 0);

            RecibeDanio(direcciondanio, 1);

        }
        if (collision.CompareTag("Respawn"))
        {
            StartCoroutine(Respawnear());
        }
        if (collision.CompareTag("PuntoRespawn"))
        {
            ultimoPuntoRespawn = collision.transform.position;
            Debug.Log("Nuevo punto de respawn activado");
            vida = 5;
            vialRegenerativo = 5;
        }
    }
}
