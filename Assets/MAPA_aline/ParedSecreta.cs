using UnityEngine;

public class ParedSecreta : MonoBehaviour
{
    public float distanciaActivacion = 4f;
    public float alturaSubida = 3.5f;
    public float velocidadSubida = 2f;

    private Transform jugador;
    private Vector3 posicionInicial;
    private Vector3 posicionAbierta;
    private bool abierta = false;

    void Start()
    {
        posicionInicial = transform.position;
        posicionAbierta = posicionInicial + Vector3.up * alturaSubida;

        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null) jugador = objPlayer.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        if (!abierta && Vector3.Distance(transform.position, jugador.position) <= distanciaActivacion)
        {
            abierta = true;
        }

        if (abierta)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionAbierta, velocidadSubida * Time.deltaTime);
        }
    }
}