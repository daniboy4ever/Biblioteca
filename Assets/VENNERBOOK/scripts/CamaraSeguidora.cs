using UnityEngine;

public class CamaraSeguir : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform objetivo; // Arrastra aquí tu Cubo jugador

    [Header("Control del Mouse")]
    public float sensibilidadMouse = 3f;
    public float distancia = 4.5f;
    public float altura = 1.6f;

    [Header("Límites verticales")]
    public float limiteInferior = -20f;
    public float limiteSuperior = 60f;

    private float rotacionX = 0f;
    private float rotacionY = 15f;

    private void Start()
    {
        // Oculta y centra el cursor como en juegos de PC
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (objetivo != null)
        {
            rotacionX = objetivo.eulerAngles.y;
        }
    }

    private void Update()
    {
        // Presiona ESC para recuperar la flecha del mouse si quieres pausar o salir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        if (objetivo == null) return;

        // Lee el movimiento del mouse
        rotacionX += Input.GetAxis("Mouse X") * sensibilidadMouse;
        rotacionY -= Input.GetAxis("Mouse Y") * sensibilidadMouse;
        rotacionY = Mathf.Clamp(rotacionY, limiteInferior, limiteSuperior);

        // Calcula la rotación y posición de la cámara detrás del cubo
        Quaternion rotacion = Quaternion.Euler(rotacionY, rotacionX, 0f);
        Vector3 puntoMira = objetivo.position + Vector3.up * altura;
        Vector3 posicionDeseada = puntoMira - (rotacion * Vector3.forward * distancia);

        transform.position = posicionDeseada;
        transform.LookAt(puntoMira);
    }
}