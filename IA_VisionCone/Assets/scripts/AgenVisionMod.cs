using UnityEngine;
using System.Collections;

public class AgentVisionMod : MonoBehaviour
{
    public Transform player;            // Transform del jugador (infiltrador)
    public float visionAngle = 30f;     // Ángulo de visión del agente
    public float visionDistance = 10f;  // Distancia máxima de visión del agente
    public float rotationInterval = 5f; // Intervalo de rotación del agente
    public float alertDuration = 5f;    // Duración del estado de alerta
    public float attackDuration = 5f;   // Duración del estado de ataque
    public float detectionTime = 3f;    // Tiempo de detección continua antes de perder al jugador

    private enum AgentState { Normal, Alert, Attack }
    private AgentState currentState = AgentState.Normal;
    private Vector3 initialPosition;
    private Vector3 lastKnownPlayerPosition;
    private bool detected = false;
    private float alertAccumulatedTime = 0f;
    private float detectionTimer = 0f;

    private Coroutine currentCoroutine;

    void Start()
    {
        initialPosition = transform.position;
        currentCoroutine = StartCoroutine(RotatePeriodically());
    }

    void Update()
    {
        switch (currentState)
        {
            case AgentState.Normal:
                NormalStateUpdate();
                break;
            case AgentState.Alert:
                AlertStateUpdate();
                break;
            case AgentState.Attack:
                AttackStateUpdate();
                break;
        }
    }

    void NormalStateUpdate()
    {
        detected = false;

        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        if (angleToPlayer < visionAngle / 2 && directionToPlayer.magnitude < visionDistance)
        {
            detected = true;
            detectionTimer += Time.deltaTime;

            if (detectionTimer >= detectionTime)
            {
                lastKnownPlayerPosition = player.position;
                currentState = AgentState.Alert;
                StopCurrentCoroutine();
                currentCoroutine = StartCoroutine(MoveToLastKnownPosition());
            }
        }
        else
        {
            detectionTimer = 0f; // Reset detection timer if player is out of sight
        }
    }

    IEnumerator MoveToLastKnownPosition()
    {
        while (currentState == AgentState.Alert)
        {
            Vector3 moveDirection = lastKnownPlayerPosition - transform.position;
            float distanceToPlayer = moveDirection.magnitude;

            if (distanceToPlayer < 0.1f)
            {
                currentState = AgentState.Normal;
                StopCurrentCoroutine();
                currentCoroutine = StartCoroutine(RotatePeriodically());
                yield break;
            }

            float moveSpeed = 2f;
            Vector3 move = moveDirection.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180f * Time.deltaTime);

            yield return null;
        }
    }

    void AlertStateUpdate()
    {
        visionAngle += 30f;

        alertAccumulatedTime += Time.deltaTime;

        if (alertAccumulatedTime >= 1f && detected)
        {
            currentState = AgentState.Attack;
            StopCurrentCoroutine();
            currentCoroutine = StartCoroutine(AttackSequence());
            return;
        }

        if (alertAccumulatedTime >= alertDuration)
        {
            currentState = AgentState.Normal;
            visionAngle = 30f;
            alertAccumulatedTime = 0f;
            StopCurrentCoroutine();
            currentCoroutine = StartCoroutine(RotatePeriodically());
        }
    }

    void AttackStateUpdate()
    {
        // El estado de ataque es manejado completamente por la corrutina AttackSequence.
    }

    IEnumerator AttackSequence()
    {
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            Vector3 moveDirection = player.position - transform.position;
            float moveSpeed = 5f;
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);

            if (moveDirection.magnitude < 0.5f)
            {
                player.gameObject.SetActive(false); // Desactivar el GameObject del infiltrador
                currentState = AgentState.Normal;
                StopCurrentCoroutine();
                currentCoroutine = StartCoroutine(RotatePeriodically());
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentState = AgentState.Normal;
        StopCurrentCoroutine();
        currentCoroutine = StartCoroutine(RotatePeriodically());
    }

    IEnumerator RotatePeriodically()
    {
        while (currentState == AgentState.Normal)
        {
            transform.Rotate(Vector3.up, 90f);
            yield return new WaitForSeconds(rotationInterval);
        }
    }

    void OnDrawGizmos()
    {
        if (visionAngle <= 0f) return;

        float halfVisionAngle = visionAngle * 0.5f;

        Vector3 p1 = PointForAngle(halfVisionAngle, visionDistance);
        Vector3 p2 = PointForAngle(-halfVisionAngle, visionDistance);

        Gizmos.color = detected ? Color.yellow : Color.blue;

        Gizmos.DrawLine(transform.position, transform.position + p1);
        Gizmos.DrawLine(transform.position, transform.position + p2);
        Gizmos.DrawRay(transform.position, transform.forward * visionDistance);
    }

    Vector3 PointForAngle(float angle, float distance)
    {
        float rad = angle * Mathf.Deg2Rad;
        return transform.TransformDirection(new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * distance);
    }

    private void StopCurrentCoroutine()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }
}
