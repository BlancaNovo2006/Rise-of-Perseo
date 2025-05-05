using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cargar escenas

public class CambiarEscena1 : MonoBehaviour
{
    public GameObject enemigoObjetivo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Estado actual de Ceto (static): " + Ceto.EstaMuerto);
            //Medusa scriptMedusa = enemigoObjetivo.GetComponent<Medusa>();

            //if (scriptMedusa.muerto == true)
            //if (scriptMedusa != null)
            //{
            //Debug.Log("Estado actual de Medusa: " + scriptMedusa.EstaMuerto);

            //scriptMedusa.EstaMuerto = true;
            //if (scriptMedusa.EstaMuerto)
            if (Ceto.EstaMuerto)
            //Debug.Log("Medusa est� muerta: " + scriptMedusa.EstaMuerto);
            {
                //Debug.Log("Cambiando a escena NivelMaritimo...");
                Debug.Log("Ceto est� muerto. Cambiando a escena Creditos...");
                SceneManager.LoadScene("Creditos");
            }
            else
            {
                Debug.Log("Ceto a�n no ha muerto");
            }
        }

    }
}