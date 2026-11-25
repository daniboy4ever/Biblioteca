using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControlJugadorCubo : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    public float velocidad = 6.0f;
    public float fuerzaSalto = 7.0f;
    public float gravedad = 20.0f;

    private CharacterController controller;
    private Vector3 direccionMovimiento = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Solo calcular movimiento en X/Z si el cubo está tocando el suelo
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal"); // Flechas Izquierda/Derecha o A/D
            float vertical = Input.GetAxis("Vertical");     // Flechas Arriba/Abajo o W/S

            direccionMovimiento = new Vector3(horizontal, 0.0f, vertical);
            direccionMovimiento = transform.TransformDirection(direccionMovimiento);
            direccionMovimiento *= velocidad;

            // Salto con la barra espaciadora
            if (Input.GetButton("Jump"))
            {
                direccionMovimiento.y = fuerzaSalto;
            }
        }

        // Aplicar la gravedad constantemente
        direccionMovimiento.y -= gravedad * Time.deltaTime;

        // Mover el CharacterController
        controller.Move(direccionMovimiento * Time.deltaTime);
    }
}