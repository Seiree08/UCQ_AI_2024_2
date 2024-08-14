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
        // Si el jugador es detectado, el agente se moverá hacia él.
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
        // Aquí puedes implementar la lógica para detectar al jugador.
        // Por simplicidad, vamos a asumir que siempre está detectado.
        // En tu proyecto, probablemente usarás raycast o conos de visión.
        float detectionRange = 10f; // Rango de detección
        float distance = Vector3.Distance(transform.position, Player.position);
        return distance <= detectionRange;
    }

    void Patrol()
    {
        // Lógica de patrullaje, por ahora el agente no hará nada.
        // Más adelante, se puede implementar un sistema de waypoints.
        _navMeshAgent.SetDestination(transform.position); // Se queda en su posición actual
    }
}
