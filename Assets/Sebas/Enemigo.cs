using UnityEngine;
using UnityEngine.AI; //Para usar IA

public class NewMonoBehaviourScript : MonoBehaviour
{
    NavMeshAgent agente;
    public Transform player;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        agente.destination = player.position;
    }

}
