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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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
    private void StartDialogue()
    {
        DialogoEmpezado = true;
        PanelDialogo.SetActive(true);
        MarcaDialogo.SetActive(false);
        lineIndex = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());
    }

    private void SiguienteLinea()
    {
        lineIndex++;
        if (lineIndex < LineasDialogo.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            DialogoEmpezado = false;
            PanelDialogo.SetActive(false);
            MarcaDialogo.SetActive(true);
            Time.timeScale = 1f;
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

