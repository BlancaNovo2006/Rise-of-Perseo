using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    public UnityEngine.UI.Image fillImage;
    public GameObject boss;
    public Transform player;
    public float mostrarBarraDistancia = 8f;

    private Medusa medusaScript;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        medusaScript = boss.GetComponent<Medusa>();
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (medusaScript == null || player == null) return;
        float distanciaAlJugador = Vector2.Distance(boss.transform.position, player.position);
        if (!medusaScript.muerto && distanciaAlJugador <= mostrarBarraDistancia)
        {
            canvasGroup.alpha = 1f;
            float fillAmount = Mathf.Clamp01((float)medusaScript.vidas / 10f);
            fillImage.fillAmount = fillAmount;
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }
}
