using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumenOpciones : MonoBehaviour
{
    public Slider slider;
    public float sliderValue = 0.5f;
    public Image imageMute;
    // Start is called before the first frame update
    void Start()
    {
        sliderValue = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        slider.SetValueWithoutNotify(sliderValue); // <- evita llamar onValueChanged automáticamente
        AudioListener.volume = sliderValue;
        RevisarSiEstoyMute();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeSlider(float valor)
    {
        sliderValue = valor;
        PlayerPrefs.SetFloat("volumenAudio", sliderValue);
        AudioListener.volume = slider.value;
        RevisarSiEstoyMute();
    }
    public void RevisarSiEstoyMute()
    {
        if(sliderValue == 0)
        {
            imageMute.enabled = true;
        }
        else
        {
            imageMute.enabled = false;
        }
    }
}
