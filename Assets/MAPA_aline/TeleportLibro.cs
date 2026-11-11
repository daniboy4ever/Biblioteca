using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeleportLibro : MonoBehaviour
{
    [Header("Configuración de Destino")]
    public Transform puntoDestino;

    [Header("Efecto de Pantalla")]
    public Image imagenOverlay;
    public float duracionTransicion = 0.25f;

    private bool procesandoTeleport = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !procesandoTeleport)
        {
            StartCoroutine(ProcesoTeletransporte(other));
        }
    }

    private IEnumerator ProcesoTeletransporte(Collider jugador)
    {
        procesandoTeleport = true;

        // 1. Pantalla Azul (Fade In)
        float tiempo = 0f;
        Color colorActual = imagenOverlay.color;
        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            colorActual.a = Mathf.Lerp(0f, 0.85f, tiempo / duracionTransicion);
            imagenOverlay.color = colorActual;
            yield return null;
        }

        // 2. Mover el jugador desactivando el CharacterController
        CharacterController cc = jugador.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        jugador.transform.position = puntoDestino.position;
        jugador.transform.rotation = puntoDestino.rotation;

        if (cc != null) cc.enabled = true;

        // 3. Volver a pantalla transparente (Fade Out)
        tiempo = 0f;
        while (tiempo < duracionTransicion)
        {
            tiempo += Time.deltaTime;
            colorActual.a = Mathf.Lerp(0.85f, 0f, tiempo / duracionTransicion);
            imagenOverlay.color = colorActual;
            yield return null;
        }

        procesandoTeleport = false;
    }
}