using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaHabilidades : MonoBehaviour
{
    public Animator animator;
    public LayerMask Enemy;
    public LayerMask capaSuelo;
    private SpriteRenderer spriteRenderer;
    MovimientoPersonaje movimientoPersonaje;

    private int habilidadActual = 0;
    private List<Habilidad> habilidades;
    public List<Image> habilidadIcons;

    public float freezeRadius = 1.5f;
    public float freezeDuration = 3f;
    public float cooldownFreezeTime = 10f;
    private bool onFreezeCooldown = false;
    public Image CooldownFreezeFill;
    public TextMeshProUGUI CooldownFreezeText;

    PegasoHabilidad pegasoHabilidad;
    private Vector3 direccionCarga;
    public float cooldownPegasoTime = 6f;
    private bool onCooldownPegaso = false;
    public Image CooldownFillPegaso;
    public TextMeshProUGUI CooldownTextPegaso;

    public int vialRegenerativo = 5;
    public Image CooldownFillVida;
    public TextMeshProUGUI CooldownTextVida;

    void Start()
    {
        ActualizarUIVidas();
        habilidades = new List<Habilidad>
        {
            new Habilidad(() => StartCoroutine(RegenerarVida()), PuedeUsarRegenerarVida),
            new Habilidad(FreezeEnemies, PuedeUsarFreezeEnemies),
            new Habilidad(Pegaso, PuedeUsarPegaso)
        };
        movimientoPersonaje = GetComponent<MovimientoPersonaje>();
        pegasoHabilidad = FindObjectOfType<PegasoHabilidad>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();


        if (CooldownFreezeFill != null)
        {
            CooldownFreezeFill.fillAmount = 1;
        }
        if (CooldownFreezeText != null)
        {
            CooldownFreezeText.text = "";
        }
        if (CooldownFillPegaso != null)
        {
            CooldownFillPegaso.fillAmount = 1;
        }
        if (CooldownTextPegaso != null)
        {
            CooldownTextPegaso.text = "";
        }
    }
    void Update()
    {
        CambiarHabilidad();
        UsarHabilidad();
        ActualizarUIHabilidad();
    }

    protected void CambiarHabilidad()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0f)
        {
            habilidadActual = (habilidadActual + 1) % habilidades.Count;
        }
        else if (scroll > 0f)
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
    protected void ActualizarUIHabilidad()
    {
        for (int i = 0; i < habilidadIcons.Count; i++)
        {
            if (i == habilidadActual)
            {
                habilidadIcons[i].color = Color.white; // Habilidad seleccionada
                habilidadIcons[i].transform.localScale = Vector3.one * 2f; // Más grande
            }
            else
            {
                habilidadIcons[i].color = new Color(1f, 1f, 1f, 0.5f); // Más opaca
                habilidadIcons[i].transform.localScale = Vector3.one; // Tamaño normal
            }
        }
    }

    // ==== CONDICIONES DE USO ====

    bool PuedeUsarFreezeEnemies() => !onFreezeCooldown;
    bool PuedeUsarPegaso() => pegasoHabilidad != null && !onCooldownPegaso;
    bool PuedeUsarRegenerarVida() => true;

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
    void FreezeEnemies()
    {
        animator.SetBool("AtaqueMedusa", true);

        Vector2 offset = new Vector2(1.5f, 0f); // Ajusta esto según la distancia que quieras delante del personaje
        bool mirandoDerecha = transform.localScale.x > 0;
        Vector2 freezePosition = (Vector2)transform.position + (mirandoDerecha ? offset : -offset);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(freezePosition, freezeRadius, Enemy);

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
            Ceto Ceto = Enemy.GetComponent<Ceto>();
            if (Ceto != null)
            {
                Ceto.Freeze(freezeDuration);
            }

        }
        Collider2D[] plataformas = Physics2D.OverlapCircleAll(freezePosition, freezeRadius, capaSuelo);

        foreach (Collider2D suelo in plataformas)
        {
            PlataformaMovil plataforma = suelo.GetComponent<PlataformaMovil>();
            if (plataforma != null)
            {
                plataforma.Freeze(freezeDuration);
            }
        }
        onFreezeCooldown = true;
        if (CooldownFreezeFill != null)
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
            if (CooldownFreezeFill != null)
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
        if (CooldownFreezeText != null)
        {
            CooldownFreezeText.text = "";
        }
        onFreezeCooldown = false;
    }
    void DesactivarMedusa()
    {
        animator.SetBool("AtaqueMedusa", false);
    }
    void Pegaso()
    {
        Debug.LogError("Función Pegaso ejecutada");
        Vector3 direccionCarga = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        pegasoHabilidad.ActivarCarga(transform.position, direccionCarga);
        //animator.SetBool("pegaso", true);
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
        if (CooldownTextPegaso != null)
        {
            CooldownTextPegaso.text = "";
        }
        onCooldownPegaso = false;
    }
    IEnumerator RegenerarVida()
    {
        if (movimientoPersonaje.vida < 10)
        {
            if (vialRegenerativo > 0)
            {
                // Cambiar el color a rojo
                spriteRenderer.color = Color.green;

                // Sumar 1 a la vida
                Debug.Log("vidaRegenerada");
                movimientoPersonaje.vida += 1;
                vialRegenerativo -= 1;
                if (vialRegenerativo < 0)
                {
                    vialRegenerativo = 0;
                }

                // Esperar 1 segundo
                yield return new WaitForSeconds(0.3f);

                // Restaurar el color a blanco
                spriteRenderer.color = Color.white;
                ActualizarUIVidas();
            }
        }

    }
    public void ActualizarUIVidas()
    {
        if (CooldownTextVida != null)
        {
            CooldownTextVida.text = "" + vialRegenerativo.ToString();
        }
    }
    void OnDrawGizmos()
    {
        Vector2 offset = new Vector2(1.5f, 0f); // Ajusta esto según la distancia que quieras delante del personaje
        bool mirandoDerecha = transform.localScale.x > 0;
        Vector2 freezePosition = (Vector2)transform.position + (mirandoDerecha ? offset : -offset);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(freezePosition, freezeRadius);
    }
}
