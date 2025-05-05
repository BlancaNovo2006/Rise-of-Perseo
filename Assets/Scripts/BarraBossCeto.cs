using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class BarraBossCeto : MonoBehaviour
{
    public UnityEngine.UI.Image fillImage;
    public GameObject boss;
    public Transform player;
    public float mostrarBarraDistancia = 8f;

    private Ceto CetoScript;
    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (CetoScript == null || player == null) return;
        float distanciaAlJugador = Vector2.Distance(boss.transform.position, player.position);
        if (!CetoScript.muerto && distanciaAlJugador <= mostrarBarraDistancia)
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
}
