using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obtaculos :  MonoBehaviour
{
    public List<GameObject> waypoint;
    public float speed = 10f;
    public float avoidanceSpeed = 5f; // Velocidad de esquivar
    public float avoidanceDistance = 2f; // Distancia para detectar obstáculos
    int index = 0;
    public bool isLoop = true;

    private Vector3 avoidanceDirection = Vector3.zero;

    void Start()
    {
        if (waypoint.Count == 0) return;
    }

    private void Update()
    {
        if (waypoint.Count == 0) return;

        Vector3 destination = waypoint[index].transform.position;
        Vector3 direction = (destination - transform.position).normalized;

        // Aplicar dirección de esquiva si es necesario
        if (avoidanceDirection != Vector3.zero)
        {
            direction += avoidanceDirection.normalized * avoidanceSpeed;
        }

        Vector3 newPosition = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        // Rotar hacia la dirección del waypoint o dirección de esquivar
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * speed);
        }

        transform.position = newPosition;

        float distance = Vector3.Distance(transform.position, destination);
        if (distance <= 0.5f)
        {
            if (index < waypoint.Count - 1)
            {
                index++;
            }
            else
            {
                if (isLoop)
                {
                    index = 0;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != waypoint[index])
        {
            // Calcular dirección de repulsión
            avoidanceDirection = transform.position - other.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != waypoint[index])
        {
            // Calcular dirección de repulsión
            avoidanceDirection = transform.position - other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != waypoint[index])
        {
            // Resetear dirección de repulsión
            avoidanceDirection = Vector3.zero;
        }
    }
}