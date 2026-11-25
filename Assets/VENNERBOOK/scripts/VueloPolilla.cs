using UnityEngine;

public class VueloPolilla : MonoBehaviour
{
    [Header("Sprites de la Polilla")]
    public Sprite polillaAlasCerradas; // polilla_1
    public Sprite polillaAlasAbiertas; // polilla_2
    public float intervaloAleteo = 0.2f;

    [Header("Vuelo Libre (Patrulla)")]
    public float velocidadNormal = 1.5f;
    public float radioVuelo = 5f; // Qué tanto abarca su zona de patrulla

    [Header("Persecución al Jugador")]
    public float distanciaDeteccion = 6f;     // A cuántos metros te detecta
    public float distanciaAbandono = 10f;     // A cuántos metros deja de seguirte si te alejas
    public float velocidadPersecucion = 3.2f; // Vuela más rápido al perseguirte
    public float distanciaMinima = 1.1f;      // Qué tan cerca llega sin meterse dentro de tu cámara
    public float alturaObjetivo = 1.2f;       // Apunta a la altura del cubo/cabeza

    [Header("Visual")]
    public bool mirarSiempreACamara = true;

    private SpriteRenderer spriteRenderer;
    private Transform jugador;
    private Vector3 puntoInicial;
    private Vector3 destinoActual;
    private float temporizadorAleteo;
    private bool usandoPrimerSprite = true;
    private bool persiguiendo = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        puntoInicial = transform.position;

        // Busca automáticamente a tu personaje por su Tag "Player"
        GameObject objJugador = GameObject.FindGameObjectWithTag("Player");
        if (objJugador != null)
        {
            jugador = objJugador.transform;
        }

        ElegirNuevoDestino();

        if (polillaAlasCerradas != null)
        {
            spriteRenderer.sprite = polillaAlasCerradas;
        }
    }

    private void Update()
    {
        // 1. Aleteo constante (aletea un poco más rápido cuando te persigue)
        float intervaloActual = persiguiendo ? intervaloAleteo * 0.6f : intervaloAleteo;
        temporizadorAleteo += Time.deltaTime;
        if (temporizadorAleteo >= intervaloActual)
        {
            temporizadorAleteo = 0f;
            usandoPrimerSprite = !usandoPrimerSprite;
            spriteRenderer.sprite = usandoPrimerSprite ? polillaAlasCerradas : polillaAlasAbiertas;
        }

        // 2. Comprueba si el jugador está cerca para iniciar o soltar la persecución
        if (jugador != null)
        {
            float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

            if (!persiguiendo && distanciaAlJugador <= distanciaDeteccion)
            {
                persiguiendo = true;
            }
            else if (persiguiendo && distanciaAlJugador >= distanciaAbandono)
            {
                persiguiendo = false;
                puntoInicial = transform.position; // Empieza a patrullar la nueva zona donde quedó
                ElegirNuevoDestino();
            }
        }

        // 3. Movimiento según si está persiguiendo o patrullando
        if (persiguiendo && jugador != null)
        {
            Vector3 posicionObjetivo = jugador.position + Vector3.up * alturaObjetivo;
            float distancia = Vector3.Distance(transform.position, posicionObjetivo);

            if (distancia > distanciaMinima)
            {
                transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidadPersecucion * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destinoActual, velocidadNormal * Time.deltaTime);
            if (Vector3.Distance(transform.position, destinoActual) < 0.2f)
            {
                ElegirNuevoDestino();
            }
        }

        // 4. Orienta el dibujo 2D hacia la cámara
        if (mirarSiempreACamara && Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    private void ElegirNuevoDestino()
    {
        Vector3 desplazamientoAzar = Random.insideUnitSphere * radioVuelo;
        desplazamientoAzar.y = Mathf.Clamp(desplazamientoAzar.y, -0.8f, 1.5f);
        destinoActual = puntoInicial + desplazamientoAzar;
    }

    // Dibuja los círculos de rango en la ventana Scene para que los ajustes visualmente
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Application.isPlaying ? puntoInicial : transform.position, radioVuelo);
    }
}