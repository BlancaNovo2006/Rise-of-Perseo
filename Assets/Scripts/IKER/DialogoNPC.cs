using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogo : MonoBehaviour
{
    [SerializeField] private GameObject MarcaDialogo;
    [SerializeField] private GameObject PanelDialogo;
    [SerializeField] private TMP_Text TextoDialogo;
    [SerializeField, TextArea(4, 6)] private string[] LineasDialogo;

    private float typingTime = 0.05f;

    private bool isPlayerInRange;
    private bool DialogoEmpezado;
    private int lineIndex;

    private AudioSource audioSource;
    public AudioClip sonidoDialogo;
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }


    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DialogoEmpezado)
            {
                StartDialogue();
            }
            else if (TextoDialogo.text == LineasDialogo[lineIndex])
            {
                SiguienteLinea();
            }
            else
            {
                StopAllCoroutines();
                TextoDialogo.text = LineasDialogo[lineIndex];
            }
        }
    }
    private void ReproducirSonidoDialogo()
    {
        if (sonidoDialogo != null)
        {
            audioSource.clip = sonidoDialogo;
            audioSource.Play();
        }
    }

    private void StartDialogue()
    {
        DialogoEmpezado = true;
        PanelDialogo.SetActive(true);
        MarcaDialogo.SetActive(false);
        lineIndex = 0;
        Time.timeScale = 0f;
        ReproducirSonidoDialogo();
        StartCoroutine(ShowLine());
    }


    private void SiguienteLinea()
    {
        audioSource.Stop(); // Detener sonido antes de continuar
        lineIndex++;

        if (lineIndex < LineasDialogo.Length)
        {
            ReproducirSonidoDialogo();
            StartCoroutine(ShowLine());
        }
        else
        {
            DialogoEmpezado = false;
            PanelDialogo.SetActive(false);
            MarcaDialogo.SetActive(true);
            Time.timeScale = 1f;
            audioSource.Stop(); // Detener sonido al finalizar diálogo
        }
    }


    private IEnumerator ShowLine()
    {
        TextoDialogo.text = string.Empty;

        foreach (char ch in LineasDialogo[lineIndex])
        {
            TextoDialogo.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            MarcaDialogo.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            MarcaDialogo.SetActive(false);
        }
    }
}

