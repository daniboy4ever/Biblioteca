using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    private NavMeshAgent agente;

    public Transform jugador;

    public float radioPatrulla = 12f; 
    public float tiempoEspera = 0.7f; //Al llegar a un punto esperará
    public float velocidadPatrulla = 3f; //Velocidad de caminata

    public float rangoDeteccion = 8f; //A esta distancia te ve
    public float rangoPerdida = 13f; //A esta distancia te deja de perseguir
    public float velocidadPersecucion = 5.7f; //Velocidad

    private float temporizadorEspera;
    private bool persiguiendo; //Qué estado tiene el enemigo

    public Animator animator;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        temporizadorEspera = tiempoEspera;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (jugador == null) return;

        //Calculamos la distancia de forma optima
        float distCuadrada = (jugador.position - transform.position).sqrMagnitude;

        if (!persiguiendo && distCuadrada <= rangoDeteccion * rangoDeteccion)
        {
            persiguiendo = true;
            agente.speed = velocidadPersecucion;
        }
        else if (persiguiendo && distCuadrada > rangoPerdida * rangoPerdida)
        {
            persiguiendo = false;
            agente.speed = velocidadPatrulla;
            agente.ResetPath();
        }

        if (persiguiendo)
        {
            agente.SetDestination(jugador.position);
        }
        else
        {
            Patrullar();
        }

        Animations();
    }

    public void Patrullar()
    {
        //Si ya llego a su destino busca otro punto aleatorio
        if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            temporizadorEspera += Time.deltaTime;
            if (temporizadorEspera >= tiempoEspera)
            {
                Vector3 puntoAleatorio = Random.insideUnitSphere * radioPatrulla + transform.position;
                if (NavMesh.SamplePosition(puntoAleatorio, out NavMeshHit hit, radioPatrulla, NavMesh.AllAreas))
                {
                    agente.SetDestination(hit.position);
                }
                temporizadorEspera = 0f;
            }
        }
    }

    //Dibuja los rangos en la escena para ajustarlos facil
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoPerdida);
    }

    public void Animations()
    {
        bool isWalking = agente.velocity.magnitude > 0.1f;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", persiguiendo && isWalking);
    }
}