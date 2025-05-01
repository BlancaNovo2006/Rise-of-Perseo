using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeteccionZonaVerticalCangrejo : MonoBehaviour
{
    public CangrejoColosal cangrejo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador entró en la ZonaVertical");
            cangrejo.ActivarZonaVertical();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador salió de la ZonaVertical");
            cangrejo.DesactivarZonaVertical();
        }
    }
}
