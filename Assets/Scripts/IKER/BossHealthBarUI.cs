using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    public Image fillImage;
    public GameObject boss;
    public Transform player;
    public float mostrarBarraDistancia = 8f;

    private Medusa medusaScript;
    private CanvasGroup canvasGroup;
    private bool bossMuerta = false;

    void Start()
    {
        medusaScript = boss.GetComponent<Medusa>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (medusaScript == null || player == null || bossMuerta) return;

        if (medusaScript.muerto || medusaScript.vidas <= 0)
        {
            OcultarBarra();
            bossMuerta = true; // No volver a mostrarla
            return;
        }

        float distanciaAlJugador = Vector2.Distance(boss.transform.position, player.position);
        if (distanciaAlJugador <= mostrarBarraDistancia)
        {
            canvasGroup.alpha = 1f;
            float fillAmount = Mathf.Clamp01((float)medusaScript.vidas / 5f);
            fillImage.fillAmount = fillAmount;
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }

    void OcultarBarra()
    {
        canvasGroup.alpha = 0f;
        fillImage.fillAmount = 0f;
    }
}
