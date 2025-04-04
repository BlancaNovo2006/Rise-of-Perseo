using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el proyectil colisiona con un objeto que no es el enemigo
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            // Destruir el proyectil
            Destroy(gameObject);
        }
    }
}
