using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movimiento2 : MonoBehaviour
{
    public float velocity = 5f;
    public float rotation = 177f;

    public Transform camara;
    private Rigidbody rb;
    private Vector3 movimientoXZ;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        adelanteCamara.y = 0;
        derechaCamara.y = 0;

        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        Vector3 direccion = (derechaCamara * horizontal + adelanteCamara * vertical);

        if (direccion.sqrMagnitude > 0.0001f)
        {
            direccion.Normalize();
        }

        movimientoXZ = direccion * velocity;
    }

    void FixedUpdate()
    {
        // Usa rb.velocity en Unity 2022 o anterior.
        // Si usas Unity 6, cambia rb.velocity por rb.linearVelocity en ambos lados:
        rb.linearVelocity = new Vector3(movimientoXZ.x, rb.linearVelocity.y, movimientoXZ.z);
    }
}