using UnityEngine;

public class RotarCirculo : MonoBehaviour
{
    [Header("Configuración de Giro")]
    public float velocidadGiro = 25f; // Giro lento (grados por segundo)

    [Header("Configuración de Latido (Escala)")]
    public float velocidadPulso = 2.5f;  // Qué tan rápido crece y vuelve
    public float aumentoMaximo = 0.15f;  // 0.15 = crece un 15% más de su tamaño original

    private Vector3 escalaOriginal;

    void Start()
    {
        // Guarda el tamaño exacto que le pusiste en el Inspector al iniciar
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        // 1. Rotar lentamente sobre su propio eje Z
        transform.Rotate(0f, 0f, velocidadGiro * Time.deltaTime, Space.Self);

        // 2. Calcular un valor que oscila suavemente entre 0 y 1 (nunca negativo)
        float onda = (Mathf.Sin(Time.time * velocidadPulso) + 1f) * 0.5f;

        // 3. Sumar el aumento a su escala original (va de 1.0x a 1.15x)
        float escalaActual = 1f + (onda * aumentoMaximo);
        transform.localScale = escalaOriginal * escalaActual;
    }
}