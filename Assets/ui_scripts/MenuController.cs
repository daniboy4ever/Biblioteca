
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelPausa;
    public GameObject panelInstrucciones;

    [Header("Temporizador de instrucciones")]
    public float tiempoInstrucciones = 8f;

    private bool juegoPausado = false;
    private bool mostrarCursor = false;
    private Coroutine temporizador;

    void Start()
    {
        panelInstrucciones.SetActive(true);
        panelPausa.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        mostrarCursor = true;

        temporizador = StartCoroutine(TemporizadorInstrucciones());
    }

    IEnumerator TemporizadorInstrucciones()
    {
        yield return new WaitForSecondsRealtime(tiempoInstrucciones);

        CerrarInstrucciones();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelInstrucciones.activeSelf)
                return;

            if (juegoPausado)
                CerrarPausa();
            else
                AbrirPausa();
        }
    }

    void LateUpdate()
    {
        if (mostrarCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // ABRIR MENU DE PAUSA
    public void AbrirPausa()
    {
        panelPausa.SetActive(true);
        juegoPausado = true;
        mostrarCursor = true;

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // CERRAR MENU DE PAUSA
    public void CerrarPausa()
    {
        panelPausa.SetActive(false);
        juegoPausado = false;
        mostrarCursor = false;

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // CERRAR INSTRUCCIONES
    public void CerrarInstrucciones()
    {
        if (!panelInstrucciones.activeSelf) return;

        if (temporizador != null)
        {
            StopCoroutine(temporizador);
            temporizador = null;
        }

        panelInstrucciones.SetActive(false);

        mostrarCursor = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // VOLVER AL MENU PRINCIPAL
    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("ui_mainmenu");
    }
}
