using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ComportamientoRaton : MonoBehaviour
{
    [Header("Salida inicial al abrir pared")]
    public Transform puntoSalidaFrente; 
    public float distanciaDeteccionSalida = 5.5f;

    [Header("Persecución (3 intentos)")]
    public float distanciaDeteccion = 3.5f;
    public float distanciaHuida = 4f;
    public float velocidad = 4.5f;
    public int vecesQueHuye = 3;

    [Header("Ajuste de postura (para que no se acueste)")]
    public Vector3 rotacionBaseModelo = Vector3.zero; // Ajusta aquí si el modelo viene acostado (ej. X = -90 o Z = 90)
    public float giroExtraY = 0f;

    [HideInInspector] public bool puedeAgarrarse = false;

    private Transform jugador;
    private Rigidbody rb;
    private Vector3 destino;
    private int huidasActuales = 0;
    private bool yaSalioAlFrente = false;
    private bool moviendose = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Congela TODA rotación física para que nunca se caiga ni se acueste

        // Guarda la postura con la que lo acomodaste en la escena
        if (rotacionBaseModelo == Vector3.zero)
        {
            rotacionBaseModelo = transform.eulerAngles;
        }

        destino = transform.position;
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null) jugador = objPlayer.transform;
    }

    void Update()
    {
        if (jugador == null || moviendose) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // 1. Primera salida: corre directo hacia PuntoSalidaRaton
        if (!yaSalioAlFrente)
        {
            if (distancia <= distanciaDeteccionSalida)
            {
                yaSalioAlFrente = true;
                if (puntoSalidaFrente != null)
                {
                    destino = new Vector3(puntoSalidaFrente.position.x, transform.position.y, puntoSalidaFrente.position.z);
                    moviendose = true;
                }
            }
            return;
        }

        // 2. Después de salir, huye 3 veces cada que te acercas
        if (distancia <= distanciaDeteccion)
        {
            if (huidasActuales < vecesQueHuye)
            {
                Vector3 direccionEscape = (transform.position - jugador.position).normalized;
                direccionEscape.y = 0f;

                // Revisa con un rayo si hay una pared enfrente antes de correr para girar a un espacio libre
                if (Physics.Raycast(transform.position + Vector3.up * 0.25f, direccionEscape, distanciaHuida))
                {
                    direccionEscape = Quaternion.Euler(0f, 120f, 0f) * direccionEscape;
                }

                destino = transform.position + direccionEscape * distanciaHuida;
                moviendose = true;
                huidasActuales++;
            }
            else
            {
                puedeAgarrarse = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!moviendose) return;

        Vector3 objetivoPlano = new Vector3(destino.x, rb.position.y, destino.z);
        Vector3 nuevaPos = Vector3.MoveTowards(rb.position, objetivoPlano, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(nuevaPos);

        // Gira solo sobre el eje horizontal (Y) respetando su postura parada
        Vector3 dir = objetivoPlano - rb.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            float anguloY = Quaternion.LookRotation(dir).eulerAngles.y + giroExtraY;
            rb.MoveRotation(Quaternion.Euler(rotacionBaseModelo.x, anguloY, rotacionBaseModelo.z));
        }

        if (Vector3.Distance(rb.position, objetivoPlano) < 0.25f)
        {
            moviendose = false;
        }
    }

    // Solo se frena si choca de lado contra una PARED (ignora el piso debajo de sus patas)
    void OnCollisionEnter(Collision col)
    {
        if (!moviendose || col.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint contacto in col.contacts)
        {
            // Si la superficie es vertical (pared) y no el suelo horizontal
            if (Mathf.Abs(contacto.normal.y) < 0.5f)
            {
                moviendose = false;
                break;
            }
        }
    }
}