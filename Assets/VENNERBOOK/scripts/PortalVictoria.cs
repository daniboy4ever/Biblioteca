using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortalVictoria : MonoBehaviour
{
    [Header("Imagen del Portal (Quad)")]
    public Transform discoPortal;
    public float velocidadGiro = 60f;
    public float pulsoPortal = 0.08f;

    [Header("Animación del Letrero (YOU WIN!)")]
    public Transform letreroFlotante;
    public float velocidadPulso = 3f;
    public float amplitudPulso = 0.08f;
    public float alturaFlote = 0.12f;

    [Header("GIF y Efecto de Victoria (Igual que TeleportLibro)")]
    public Image imagenOverlay;
    public float duracionEfecto = 3f;
    public float duracionFade = 0.5f;
    public float zoomFinal = 1.15f;
    public Sprite[] framesGif;
    public float cuadrosPorSegundo = 18f;

    [Header("Audio del Túnel")]
    public AudioClip sonidoTeleport;
    [Range(0f, 1f)]
    public float volumenSonido = 0.8f;

    private AudioSource fuenteAudio;
    private Vector3 escalaInicialLetrero;
    private Vector3 posInicialLetrero;
    private Vector3 escalaInicialPortal;
    private bool activado = false;

    void Start()
    {
        if (letreroFlotante != null)
        {
            escalaInicialLetrero = letreroFlotante.localScale;
            posInicialLetrero = letreroFlotante.localPosition;
        }
        if (discoPortal != null)
        {
            escalaInicialPortal = discoPortal.localScale;
        }

        fuenteAudio = GetComponent<AudioSource>();
        if (fuenteAudio == null)
        {
            fuenteAudio = gameObject.AddComponent<AudioSource>();
        }
        fuenteAudio.playOnAwake = false;
    }

    void Update()
    {
        if (discoPortal != null)
        {
            discoPortal.Rotate(0f, 0f, velocidadGiro * Time.deltaTime);
            float factorPortal = 1f + Mathf.Sin(Time.time * velocidadPulso) * pulsoPortal;
            discoPortal.localScale = escalaInicialPortal * factorPortal;
        }

        if (letreroFlotante != null)
        {
            float pulso = 1f + Mathf.Sin(Time.time * velocidadPulso) * amplitudPulso;
            letreroFlotante.localScale = escalaInicialLetrero * pulso;

            float offsetY = Mathf.Sin(Time.time * (velocidadPulso * 0.8f)) * alturaFlote;
            letreroFlotante.localPosition = posInicialLetrero + new Vector3(0f, offsetY, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !activado)
        {
            StartCoroutine(ProcesoVictoria(other.gameObject));
        }
    }

    private IEnumerator ProcesoVictoria(GameObject jugador)
    {
        activado = true;

        // 1. Congela el movimiento de Charlie al ganar
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

        // 3. Enciende el overlay desde alpha 0
        if (imagenOverlay != null)
        {
            Color c = imagenOverlay.color;
            c.a = 0f;
            imagenOverlay.color = c;
            imagenOverlay.rectTransform.localScale = Vector3.one;
            imagenOverlay.gameObject.SetActive(true);
        }

        // 4. Reproduce los frames del GIF con fade y zoom
        float tiempoTranscurrido = 0f;
        float tiempoPorFrame = 1f / cuadrosPorSegundo;
        float temporizadorFrame = 0f;
        int indiceFrame = 0;
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

                Color c = imagenOverlay.color;
                c.a = Mathf.Clamp01(alpha);
                imagenOverlay.color = c;

                float progreso = tiempoTranscurrido / duracionEfecto;
                float escala = Mathf.Lerp(1f, zoomFinal, progreso);
                imagenOverlay.rectTransform.localScale = new Vector3(escala, escala, 1f);
            }

            yield return null;
        }
    }
}