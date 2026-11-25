using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VidaJugador : MonoBehaviour
{
    public int vidas = 3;
    public Image[] corazones; //Aquí arrastras tus 3 imágenes

    public float tiempoInvulnerable = 1.7f; //Para que quite los 3 corazones de golpe
    private float temporizadorInvulnerable;

    void Update()
    {
        if (temporizadorInvulnerable > 0)
        {
            temporizadorInvulnerable -= Time.deltaTime; //Baje el tiempo
        }
    }

    public void RecibirDano() //Será llamada por la bibliotecaria
    {
        //No recibir daño mientras haya invulnerabilidad
        if (temporizadorInvulnerable > 0) return;

        vidas--;
        temporizadorInvulnerable = tiempoInvulnerable;

        ActualizarCorazones();

        if (vidas <= 0)
        {
            //Reiniciar nivel actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ActualizarCorazones()
    {
        for (int i = 0; i < corazones.Length; i++)
        {
            //Activa el corazón si su posición es menor a las vidas actuales
            corazones[i].enabled = i < vidas;
        }
    }
}