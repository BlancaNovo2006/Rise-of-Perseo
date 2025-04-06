using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonSalir : MonoBehaviour
{
    public void CerrarJuego()
    {
        Debug.Log("Cerrando el juego..."); // Esto solo se ve en el editor
        Application.Quit();
    }
}
