using UnityEngine;

public class VueloPolilla : MonoBehaviour
{
    [Header("Sprites de la Polilla")]
    public Sprite polillaAlasCerradas; // polilla_1
    public Sprite polillaAlasAbiertas; // polilla_2
    [Tooltip("Tiempo en segundos entre cada aleteo (ej. 0.15 para aleteo rápido, 1 para cada segundo)")]
    public float intervaloAleteo = 0.2f;

    [Header("Movimiento en la Biblioteca")]
    public float velocidadVuelo = 1.5f;
    public float radioVuelo = 3f;
    public bool mirarSiempreACamara = true;

    private SpriteRenderer spriteRenderer;
    private Vector3 puntoInicial;
    private Vector3 destinoActual;
    private float temporizadorAleteo;
    private bool usandoPrimerSprite = true;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        puntoInicial = transform.position;
        ElegirNuevoDestino();

        if (polillaAlasCerradas != null)
        {
            spriteRenderer.sprite = polillaAlasCerradas;
        }
    }

    private void Update()
    {
        // 1. Cambia entre la imagen 1 y la imagen 2 cada cierto tiempo
        temporizadorAleteo += Time.deltaTime;
        if (temporizadorAleteo >= intervaloAleteo)
        {
            temporizadorAleteo = 0f;
            usandoPrimerSprite = !usandoPrimerSprite;
            spriteRenderer.sprite = usandoPrimerSprite ? polillaAlasCerradas : polillaAlasAbiertas;
        }

        // 2. Mueve la polilla suavemente por el aire alrededor de su zona
        transform.position = Vector3.MoveTowards(transform.position, destinoActual, velocidadVuelo * Time.deltaTime);
        if (Vector3.Distance(transform.position, destinoActual) < 0.1f)
        {
            ElegirNuevoDestino();
        }

        // 3. Hace que el dibujo 2D siempre mire hacia tu cámara para no verse plano de lado
        if (mirarSiempreACamara && Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

    private void ElegirNuevoDestino()
    {
        Vector3 desplazamientoAzar = Random.insideUnitSphere * radioVuelo;
        desplazamientoAzar.y = Mathf.Clamp(desplazamientoAzar.y, -0.5f, 1.2f);
        destinoActual = puntoInicial + desplazamientoAzar;
    }
}