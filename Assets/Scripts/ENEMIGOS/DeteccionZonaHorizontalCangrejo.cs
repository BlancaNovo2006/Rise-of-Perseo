using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeteccionZonaHorizontalCangrejo : MonoBehaviour
{
    public CangrejoColosal cangrejo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador entró en la ZonaHorizontal");
            cangrejo.ActivarZonaHorizontal();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jugador salió de la ZonaHorizontal");
            cangrejo.DesactivarZonaHorizontal();
        }
    }
}
