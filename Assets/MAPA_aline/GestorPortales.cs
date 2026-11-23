using System.Collections.Generic;
using UnityEngine;

public class GestorPortales : MonoBehaviour
{
    [Header("Lista de escondites en la biblioteca")]
    public Transform[] lugaresPosibles;

    [Header("Los 3 libros portal")]
    public Transform[] portalesActivos;

    // Guarda qué lugar está ocupando cada portal actualmente
    private Dictionary<Transform, int> ocupados = new Dictionary<Transform, int>();

    private void Start()
    {
        // Al iniciar el juego, reparte los 3 portales en lugares distintos al azar
        foreach (Transform portal in portalesActivos)
        {
            if (portal != null)
            {
                MoverPortalAlAzar(portal);
            }
        }
    }

    public void MoverPortalAlAzar(Transform portal)
    {
        if (lugaresPosibles == null || lugaresPosibles.Length == 0) return;

        // Busca todos los lugares que NO estén ocupados por otro portal ni por este mismo
        List<int> indicesLibres = new List<int>();
        for (int i = 0; i < lugaresPosibles.Length; i++)
        {
            if (lugaresPosibles[i] != null && !ocupados.ContainsValue(i))
            {
                indicesLibres.Add(i);
            }
        }

        if (indicesLibres.Count == 0) return;

        // Elige un escondite libre al azar
        int elegido = indicesLibres[Random.Range(0, indicesLibres.Count)];
        ocupados[portal] = elegido;

        // Mueve el libro y su círculo mágico al nuevo escondite
        portal.position = lugaresPosibles[elegido].position;
        portal.rotation = lugaresPosibles[elegido].rotation;
    }
}