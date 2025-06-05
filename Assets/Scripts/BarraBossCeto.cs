using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraBossCeto : MonoBehaviour
{
    public Image fillImage;
    public GameObject boss;
    public Transform player;
    public float mostrarBarraDistancia = 8f;

    private Ceto CetoScript;
    private CanvasGroup canvasGroup;
    private bool bossMuerto = false;

    void Start()
    {
        CetoScript = boss.GetComponent<Ceto>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (CetoScript == null || player == null || bossMuerto) return;

        if (CetoScript.muerto || CetoScript.vidas <= 0)
        {
            OcultarBarra();
            bossMuerto = true; // No volver a mostrarla
            return;
        }

        float distanciaAlJugador = Vector2.Distance(boss.transform.position, player.position);
        if (distanciaAlJugador <= mostrarBarraDistancia)
        {
            canvasGroup.alpha = 1f;
            float fillAmount = Mathf.Clamp01((float)CetoScript.vidas / 10f);
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
