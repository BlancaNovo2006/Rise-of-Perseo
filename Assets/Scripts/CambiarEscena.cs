using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public GameObject enemigoObjetivo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Estado actual de Medusa (static): " + Medusa.EstaMuerto);
            //Medusa scriptMedusa = enemigoObjetivo.GetComponent<Medusa>();

            //if (scriptMedusa.muerto == true)
            //if (scriptMedusa != null)
            //{
            //Debug.Log("Estado actual de Medusa: " + scriptMedusa.EstaMuerto);

            //scriptMedusa.EstaMuerto = true;
            //if (scriptMedusa.EstaMuerto)
            if (Medusa.EstaMuerto)
            //Debug.Log("Medusa está muerta: " + scriptMedusa.EstaMuerto);
            {
                //Debug.Log("Cambiando a escena NivelMaritimo...");
                Debug.Log("Medusa está muerta. Cambiando a escena NivelMaritimo...");
                SceneManager.LoadScene("NivelMaritimo");//cambiar a nivel maritimo despues de DEMODAY
            }
            else
            {
                    Debug.Log("Medusa aún no ha muerto");
            }
        }   
        
    }
}
