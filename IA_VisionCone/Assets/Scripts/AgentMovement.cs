using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    public Transform Player; // Asignaremos la referencia del jugador desde el Inspector.
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        // Obtenemos el componente NavMeshAgent del objeto.
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Si el jugador es detectado, el agente se mover� hacia �l.
        if (PlayerDetected())
        {
            _navMeshAgent.SetDestination(Player.position);
        }
        else
        {
            // Implementar comportamiento de patrullaje cuando no se detecta al jugador.
            Patrol();
        }
    }

    bool PlayerDetected()
    {
        // Aqu� puedes implementar la l�gica para detectar al jugador.
        // Por simplicidad, vamos a asumir que siempre est� detectado.
        // En tu proyecto, probablemente usar�s raycast o conos de visi�n.
        float detectionRange = 10f; // Rango de detecci�n
        float distance = Vector3.Distance(transform.position, Player.position);
        return distance <= detectionRange;
    }

    void Patrol()
    {
        // L�gica de patrullaje, por ahora el agente no har� nada.
        // M�s adelante, se puede implementar un sistema de waypoints.
        _navMeshAgent.SetDestination(transform.position); // Se queda en su posici�n actual
    }
}
