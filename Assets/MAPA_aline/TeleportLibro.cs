using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeleportLibro : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    public Transform puntoDestino;
    public GestorPortales gestorPortales;
    public Image imagenOverlay;
    public float duracionEfecto = 3f;

    [Header("Suavizado (Difuminado y Zoom)")]
    public float duracionFade = 0.5f;
    public float zoomFinal = 1.15f;

    [Header("Animación del GIF (Frames)")]
    public Sprite[] framesGif;
    public float cuadrosPorSegundo = 18f;

    [Header("Audio del Túnel")]
    public AudioClip sonidoTeleport;
    [Range(0f, 1f)]
    public float volumenSonido = 0.8f;

    private AudioSource fuenteAudio;
    private bool teletransportando = false;

    private void Start()
    {
        if (imagenOverlay != null)
        {
            imagenOverlay.gameObject.SetActive(false);
        }

        // Prepara el reproductor de audio automáticamente en el libro
        fuenteAudio = GetComponent<AudioSource>();
        if (fuenteAudio == null)
        {
            fuenteAudio = gameObject.AddComponent<AudioSource>();
        }
        fuenteAudio.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !teletransportando)
        {
            StartCoroutine(ProcesoTeleport(other.gameObject));
        }
    }

    private IEnumerator ProcesoTeleport(GameObject jugador)
    {
        if (gestorPortales != null)
{
    gestorPortales.MoverPortalAlAzar(this.transform);
}
        teletransportando = true;

        // 1. Pausa el movimiento y congela al jugador
        Movimiento2 mov = jugador.GetComponent<Movimiento2>();
        if (mov != null) mov.enabled = false;

        Rigidbody rb = jugador.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = true;
        }

        // 2. Inicia el sonido del túnel
        if (fuenteAudio != null && sonidoTeleport != null)
        {
            fuenteAudio.clip = sonidoTeleport;
            fuenteAudio.volume = volumenSonido;
            fuenteAudio.Play();
        }

        // 3. Prepara la imagen transparente antes de encenderla
        if (imagenOverlay != null)
        {
            Color c = imagenOverlay.color;
            c.a = 0f;
            imagenOverlay.color = c;
            imagenOverlay.rectTransform.localScale = Vector3.one;
            imagenOverlay.gameObject.SetActive(true);
        }

        // 4. Reproduce el GIF con difuminado, zoom y desvanecimiento de audio
        float tiempoTranscurrido = 0f;
        float tiempoPorFrame = 1f / cuadrosPorSegundo;
        float temporizadorFrame = 0f;
        int indiceFrame = 0;
        bool yaTeletransportado = false;
        float fadeSeguro = Mathf.Min(duracionFade, duracionEfecto * 0.45f);

        if (framesGif != null && framesGif.Length > 0 && imagenOverlay != null)
        {
            imagenOverlay.sprite = framesGif[0];
        }

        while (tiempoTranscurrido < duracionEfecto)
        {
            tiempoTranscurrido += Time.deltaTime;
            temporizadorFrame += Time.deltaTime;

            if (framesGif != null && framesGif.Length > 0 && temporizadorFrame >= tiempoPorFrame)
            {
                temporizadorFrame -= tiempoPorFrame;
                indiceFrame = (indiceFrame + 1) % framesGif.Length;
                imagenOverlay.sprite = framesGif[indiceFrame];
            }

            if (imagenOverlay != null)
            {
                float alpha = 1f;
                if (tiempoTranscurrido < fadeSeguro)
                {
                    alpha = tiempoTranscurrido / fadeSeguro;
                }
                else if (tiempoTranscurrido > duracionEfecto - fadeSeguro)
                {
                    alpha = (duracionEfecto - tiempoTranscurrido) / fadeSeguro;

                    // Baja también el volumen suavemente al salir del túnel
                    if (fuenteAudio != null && fuenteAudio.isPlaying)
                    {
                        fuenteAudio.volume = Mathf.Clamp01(alpha) * volumenSonido;
                    }
                }

                Color c = imagenOverlay.color;
                c.a = Mathf.Clamp01(alpha);
                imagenOverlay.color = c;

                float progreso = tiempoTranscurrido / duracionEfecto;
                float escala = Mathf.Lerp(1f, zoomFinal, progreso);
                imagenOverlay.rectTransform.localScale = new Vector3(escala, escala, 1f);
            }

            if (!yaTeletransportado && tiempoTranscurrido >= duracionEfecto * 0.5f)
            {
                yaTeletransportado = true;
                MoverJugador(jugador, rb);
            }

            yield return null;
        }

        // 5. Detiene el sonido, apaga el Canvas y devuelve el control
        if (fuenteAudio != null && fuenteAudio.isPlaying)
        {
            fuenteAudio.Stop();
        }

        if (imagenOverlay != null)
        {
            imagenOverlay.rectTransform.localScale = Vector3.one;
            imagenOverlay.gameObject.SetActive(false);
        }

        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (mov != null) mov.enabled = true;

        teletransportando = false;
    }

    private void MoverJugador(GameObject jugador, Rigidbody rb)
    {
        if (puntoDestino == null) return;

        jugador.transform.position = puntoDestino.position;
        jugador.transform.rotation = puntoDestino.rotation;

        if (rb != null)
        {
            rb.position = puntoDestino.position;
        }
    }
}