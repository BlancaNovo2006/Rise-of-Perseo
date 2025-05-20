using UnityEngine;

public class Columna : MonoBehaviour
{
    public float alturaObjetivo = 2f;
    public float velocidadMovimiento = 5f;
    private Vector3 posicionInicial;
    private Vector3 posicionFinal;
    private bool irArriba = false;
    private bool irAbajo = false;

    void Start()
    {
        posicionInicial = transform.position;
        posicionFinal = posicionInicial + Vector3.up * alturaObjetivo;
    }

    void Update()
    {
        if (irArriba)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionFinal, velocidadMovimiento * Time.deltaTime);
        }
        else if (irAbajo)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionInicial, velocidadMovimiento * Time.deltaTime);
        }
    }

    public void Activar()
    {
        irArriba = true;
        irAbajo = false;
    }

    public void VolverAbajo()
    {
        irArriba = false;
        irAbajo = true;
    }
}
