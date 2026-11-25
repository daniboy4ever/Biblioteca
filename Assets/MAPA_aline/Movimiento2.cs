using UnityEngine;

public class Movimiento2 : MonoBehaviour
{
    public float velocidad = 6f;
    public float velocidadGiro = 12f;
    private Animator anim;
    private Transform camara;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (Camera.main != null)
        {
            camara = Camera.main.transform;
        }
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal"); // A y D (izquierda / derecha de la pantalla)
        float z = Input.GetAxis("Vertical");   // W y S (adelante / atrás de la cámara)

        Vector3 direccion;

        if (camara != null)
        {
            // Toma hacia dónde mira la cámara en el plano horizontal (sin inclinación arriba/abajo)
            Vector3 frenteCamara = camara.forward;
            Vector3 derechaCamara = camara.right;
            frenteCamara.y = 0f;
            derechaCamara.y = 0f;
            frenteCamara.Normalize();
            derechaCamara.Normalize();

            direccion = (frenteCamara * z + derechaCamara * x).normalized;
        }
        else
        {
            direccion = new Vector3(x, 0f, z).normalized;
        }

        if (direccion.magnitude >= 0.1f)
        {
            transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);

            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.deltaTime);

            if (anim != null) anim.SetBool("isRunning", true);
        }
        else
        {
            if (anim != null) anim.SetBool("isRunning", false);
        }
    }
}