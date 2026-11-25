using UnityEngine;
using UnityEngine.UI;

public class ObjetoRecogible : MonoBehaviour
{
    public enum TipoArtefacto { Libro, Raton, Llave }
    public TipoArtefacto tipo;

    [Header("Interfaz y Secuencia")]
    public Image circuloUI;             // Su círculo en el Canvas
    public GameObject siguienteObjeto;  // En Libro -> arrastra Raton | En Raton -> arrastra Llave
    public GameObject puertaOficina;    // En Llave -> arrastra la puerta de la oficina
    public float distanciaAgarre = 2.5f;

    private Transform jugador;
    private ComportamientoRaton scriptRaton;

    void Start()
    {
        scriptRaton = GetComponent<ComportamientoRaton>();
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null) jugador = objPlayer.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        // Si es el ratón y aún no huye sus 3 veces, no deja agarrarlo
        if (tipo == TipoArtefacto.Raton && scriptRaton != null && !scriptRaton.puedeAgarrarse)
            return;

        if (Vector3.Distance(transform.position, jugador.position) <= distanciaAgarre)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Recoger();
            }
        }
    }

    void Recoger()
    {
        // 1. Ilumina su círculo a todo color
        if (circuloUI != null)
        {
            circuloUI.color = Color.white;
        }

        // 2. Hace aparecer el siguiente artefacto de la cadena
        if (siguienteObjeto != null)
        {
            siguienteObjeto.SetActive(true);
        }

        // 3. Si es el último (la Llave), abre la oficina de la bibliotecaria
        if (puertaOficina != null)
        {
            puertaOficina.SetActive(false);
        }

        Destroy(gameObject);
    }
}