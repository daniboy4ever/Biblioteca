using UnityEngine;

public class Movimiento2 : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 6f;
    public float velocidadGiro = 12f;
    public float fuerzaSalto = 5f;

    private Rigidbody rb;
    private Transform camaraPrincipal;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Evita que el cubo ruede como dado al chocar

        if (Camera.main != null)
        {
            camaraPrincipal = Camera.main.transform;
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // A y D
        float vertical = Input.GetAxisRaw("Vertical");     // W y S

        Vector3 direccionInput = new Vector3(horizontal, 0f, vertical).normalized;

        if (direccionInput.magnitude >= 0.1f && camaraPrincipal != null)
        {
            // Calcula el ángulo hacia donde apunta la cámara + las teclas presionadas
            float anguloObjetivo = Mathf.Atan2(direccionInput.x, direccionInput.z) * Mathf.Rad2Deg + camaraPrincipal.eulerAngles.y;
            Quaternion rotacionSuave = Quaternion.Euler(0f, anguloObjetivo, 0f);

            // Gira el cubo suavemente hacia donde camina
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, rotacionSuave, velocidadGiro * Time.fixedDeltaTime));

            // Mueve el cubo hacia adelante en esa dirección sin alterar su caída/gravedad
            Vector3 direccionMovimiento = rotacionSuave * Vector3.forward;
            rb.linearVelocity = new Vector3(direccionMovimiento.x * velocidad, rb.linearVelocity.y, direccionMovimiento.z * velocidad);
        }
        else
        {
            // Frena el desplazamiento horizontal al soltar las teclas
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    private void Update()
    {
        // Salto con barra espaciadora
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.05f)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }
    }
}