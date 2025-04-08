using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cargar escenas

public class CambiarEscena : MonoBehaviour
{
    // Este método se llama cuando otro collider entra en el trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Aquí puedes comprobar si el objeto que entra al trigger es el correcto, por ejemplo si es el jugador
        if (other.CompareTag("Player"))
        {
            // Cambiar a la escena con el nombre especificado
            SceneManager.LoadScene("Creditos");
        }
    }
}
