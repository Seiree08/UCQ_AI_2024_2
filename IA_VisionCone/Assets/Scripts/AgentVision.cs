//Este còdigo està basado en el video de youtube proporcionado por el profesor
//https://youtu.be/lV47ED8h61k?si=6m012cxUMIkJvd5z

//Se usó la página de global explorer
//https://explorer.globe.engineer/

// Importar las librerías necesarias
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentVision : MonoBehaviour
{
    public Transform Infiltrator;  // Transform del jugador que el agente está buscando
    public Transform Agent;  // Transform de la cabeza del agente (punto de referencia para la visión)

    [Range(0f, 360f)]
    public float VisionAngle = 30f;  // Rango del ángulo de visión del agente
    public float VisionDistance = 10f;  // Distancia máxima de visión del agente

    private bool detected = false;  // Variable para almacenar si el jugador ha sido detectado

    //El giro del agente
    private float targetRotation = 0f;  // Ángulo de rotación objetivo
    public float rotationSpeed = 45f;  // Velocidad de rotación en grados por segundo

    private Coroutine rotationCoroutine;  // Referencia a la corrutina de rotación

    private void Start()
    {
        // Inicia la rotación del agente
        StartRotation();
    }

    private void Update()
    {
        // Reinicia la detección
        detected = false;

        // Vector desde la cabeza del agente hasta el jugador
        Vector3 PlayerVector = Infiltrator.position - Agent.position;

        // Comprueba si el jugador está dentro del ángulo de visión
        if (Vector3.Angle(PlayerVector, Agent.forward) < VisionAngle * 0.5f)
        {
            // Comprueba si el jugador está dentro de la distancia de visión
            if (PlayerVector.magnitude < VisionDistance)
            {
                // Si ambas condiciones se cumplen, el jugador es detectado
                detected = true;
            }
        }

        // Si el jugador es detectado, detén el giro
        if (detected)
        {
            StopRotation();
        }
        else
        {
            // Si no es detectado, continúa girando
            StartRotation();  // Asegura que la rotación esté activa
        }
    }

    private void OnDrawGizmos()
    {
        // Si el ángulo de visión es menor o igual a 0, no hacer nada
        if (VisionAngle <= 0f) return;

        // Calcular la mitad del ángulo de visión
        float HalfVisionAngle = VisionAngle * 0.5f;

        // Calcular los puntos en los extremos del ángulo de visión
        Vector3 p1 = PointForAngle(HalfVisionAngle, VisionDistance);
        Vector3 p2 = PointForAngle(-HalfVisionAngle, VisionDistance);

        // Cambiar el color del Gizmo según si el jugador ha sido detectado o no
        Gizmos.color = detected ? Color.yellow : Color.blue;

        // Dibujar las líneas del cono de visión
        Gizmos.DrawLine(Agent.position, Agent.position + p1);
        Gizmos.DrawLine(Agent.position, Agent.position + p2);
        // Dibujar una línea central hacia adelante desde la cabeza
        Gizmos.DrawRay(Agent.position, Agent.forward * VisionDistance);
    }

    // Método para calcular un punto en el espacio 3D dado un ángulo y una distancia
    Vector3 PointForAngle(float Angle, float Distance)
    {
        // Convertir el ángulo a radianes
        float rad = Angle * Mathf.Deg2Rad;
        // Calcular y devolver el punto usando seno y coseno
        return Agent.TransformDirection(new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * Distance);
    }

    // Método para iniciar la rotación del agente hacia el ángulo objetivo
    private void StartRotation()
    {
        if (rotationCoroutine == null)
        {
            // Calcula el ángulo objetivo sumando 90 grados al ángulo actual
            targetRotation = Agent.eulerAngles.y + 90f;
            rotationCoroutine = StartCoroutine(RotateToTarget());
        }
    }

    // Método para detener la rotación del agente
    public void StopRotation()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }

    // Corrutina para rotar el agente hacia el ángulo objetivo
    private IEnumerator RotateToTarget()
    {
        float startRotation = Agent.eulerAngles.y;
        float timer = 0f;

        while (timer < 2f)  // Rotar durante 2 segundos
        {
            float angle = Mathf.Lerp(startRotation, targetRotation, timer / 2f);
            Agent.rotation = Quaternion.Euler(0, angle, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        // Asegura que el agente termine exactamente en el ángulo objetivo
        Agent.rotation = Quaternion.Euler(0, targetRotation, 0);

        // Espera un breve momento antes de reiniciar la rotación si no se ha detenido
        yield return new WaitForSeconds(3f);

        if (detected == false)
        {
            // Reinicia la rotación si no se ha detectado al jugador
            StartRotation();
        }
        else
        {
            rotationCoroutine = null;
        }
    }
}
