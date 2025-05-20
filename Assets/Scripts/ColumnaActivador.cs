using UnityEngine;

public class ColumnaActivador : MonoBehaviour
{
    private Columna columna;
    private bool yaActivado = false;

    void Start()
    {
        columna = GetComponentInParent<Columna>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!yaActivado && other.CompareTag("Player"))
        {
            columna.Activar();
            yaActivado = true;
        }
    }
}
