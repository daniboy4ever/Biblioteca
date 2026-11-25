using UnityEngine;

public class SaltoVariable : MonoBehaviour
{
    public float fuerzaSaltoNormal = 5.5f;   // Para escalones (toque rápido)
    public float fuerzaSaltoAlto = 8.5f;     // Para sillas/mesas (mantener Space)
    public float tiempoCargaMax = 0.25f;     // Segundos manteniendo Space para salto alto
    public float distanciaSuelo = 0.2f;

    private Rigidbody rb;
    private CharacterController controller;
    private float velocidadY;
    private float tiempoPresionado = 0f;
    private bool cargandoSalto = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool enSuelo = EstaEnSuelo();

        if (enSuelo && velocidadY < 0)
            velocidadY = -2f;

        // Empieza a presionar Space
        if (enSuelo && Input.GetKeyDown(KeyCode.Space))
        {
            cargandoSalto = true;
            tiempoPresionado = 0f;
        }

        // Mientras mantiene presionado Space
        if (cargandoSalto && Input.GetKey(KeyCode.Space))
        {
            tiempoPresionado += Time.deltaTime;
            if (tiempoPresionado >= tiempoCargaMax)
            {
                EjecutarSalto(fuerzaSaltoAlto);
                cargandoSalto = false;
            }
        }

        // Si suelta Space antes de tiempo, da el salto normal
        if (cargandoSalto && Input.GetKeyUp(KeyCode.Space))
        {
            EjecutarSalto(fuerzaSaltoNormal);
            cargandoSalto = false;
        }

        // Gravedad si usa CharacterController
        if (controller != null)
        {
            velocidadY += Physics.gravity.y * 2f * Time.deltaTime;
            controller.Move(Vector3.up * velocidadY * Time.deltaTime);
        }
    }

    void EjecutarSalto(float fuerza)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerza, rb.linearVelocity.z);
        }
        else if (controller != null)
        {
            velocidadY = fuerza;
        }
    }

    bool EstaEnSuelo()
    {
        if (controller != null) return controller.isGrounded;
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, distanciaSuelo + 0.1f);
    }
}