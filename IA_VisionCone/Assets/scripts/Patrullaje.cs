using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrullaje : MonoBehaviour
{
    public List<GameObject> waypoint;
    public float speed = 20f;
    int index = 0;
    public bool isLoop = true;

    void Start()
    {

    }

    private void Update()
    {
        if (waypoint.Count == 0) return;

        Vector3 destination = waypoint[index].transform.position;
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        // Rotar hacia la dirección del waypoint
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
}