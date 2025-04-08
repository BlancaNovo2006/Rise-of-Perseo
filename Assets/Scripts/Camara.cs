using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camara : MonoBehaviour
{
    public Transform target; //para poner q siga al personaje
    public Vector3 offset = new Vector3(0f, 5f, -10f); //movimiento de la camara
    public float smoothSpeed = 0.125f; // Velocidad de suavizado
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target no asignado a la c�mara");
            return;
        }

        Vector3 desiredPosition = target.position + offset; //posicion a la q quiere ir

        // Interpolaci�n suave entre la posici�n actual y la deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;// posici�n suavizada a la c�mara (arregla que no tambalee)

        transform.LookAt(target); //para q la camara mire al personaje
    }
}
