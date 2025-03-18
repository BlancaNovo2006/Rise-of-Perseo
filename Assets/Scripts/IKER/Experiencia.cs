using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiencia : MonoBehaviour
{
    public int cantidadExperiencia = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientoPersonaje player = other.GetComponent<MovimientoPersonaje>();
            if (player != null) 
            {
                player.AgregarExperiencia(cantidadExperiencia);
                Destroy(gameObject);
            }
        }
    }
}
