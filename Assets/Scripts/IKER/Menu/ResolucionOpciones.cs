using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolucionOpciones : MonoBehaviour
{
    public TMP_Dropdown resolucionesDropDown;
    Resolution[] resoluciones;
    // Start is called before the first frame update
    void Start()
    {
        RevisarResolucion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionesDropDown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + "x" + resoluciones[i].height;
            opciones.Add(opcion);

            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }
        }

        resolucionesDropDown.AddOptions(opciones);

        // Recuperar índice guardado (si existe)
        int indiceGuardado = PlayerPrefs.GetInt("numeroResolucion", resolucionActual);
        resolucionesDropDown.value = indiceGuardado;
        resolucionesDropDown.RefreshShownValue();

        // Aplicar la resolución guardada
        Resolution resolucion = resoluciones[indiceGuardado];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }
    public void CambiarResolucion(int indiceResolucion)
    {
        PlayerPrefs.SetInt("numeroResolucion", resolucionesDropDown.value);
        Resolution resolucion = resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }
}
