using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private MovimientoPersonaje movimientoPersonaje;
    private float vidaMaxima;
    // Start is called before the first frame update
    void Start()
    {
        movimientoPersonaje = GameObject.Find("Personaje").GetComponent<MovimientoPersonaje>();
        vidaMaxima = movimientoPersonaje.vida;
    }

    // Update is called once per frame
    void Update()
    {
        rellenoBarraVida.fillAmount = movimientoPersonaje.vida / vidaMaxima;
    }
}
