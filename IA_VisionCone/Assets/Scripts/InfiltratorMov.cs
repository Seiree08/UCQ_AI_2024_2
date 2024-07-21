using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiltratorMov : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float slowingDistance = 2f;
    private Vector3 targetPosition;
    private bool isMoving = false;


    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                targetPosition = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
                isMoving = true;
            }
        }

        if (isMoving)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        Vector3 toTarget = targetPosition - transform.position;
        float distance = toTarget.magnitude;

        if (distance < 0.1f)
        {
            isMoving = false;
            return;
        }

        float speed = maxSpeed;
        if (distance < slowingDistance)
        {
            speed = maxSpeed * (distance / slowingDistance);
        }

        Vector3 move = toTarget.normalized * speed * Time.deltaTime;
        transform.position += new Vector3(move.x, 0, move.z);
    }
}