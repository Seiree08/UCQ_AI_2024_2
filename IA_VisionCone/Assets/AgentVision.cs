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
    // Transform del jugador que el agente está buscando
    public Transform Player;
    // Transform de la cabeza del agente (el punto de referencia para la visión)
    public Transform Head;

    // Rango del ángulo de visión del agente (de 0 a 360 grados)
    [Range(0f, 360f)]
    public float VisionAngle = 30f;
    // Distancia máxima de visión del agente
    public float VisionDistance = 10f;

    // Variable para almacenar si el jugador ha sido detectado
    bool detected = false;

    // Método que se llama en cada frame
    private void Update()
    {
        // Reiniciar la detección
        detected = false;
        // Vector desde la cabeza del agente hasta el jugador
        Vector3 PlayerVector = Player.position - Head.position;

        // Comprobar si el jugador está dentro del ángulo de visión
        if (Vector3.Angle(PlayerVector, Head.forward) < VisionAngle * 0.5f)
        {
            // Comprobar si el jugador está dentro de la distancia de visión
            if (PlayerVector.magnitude < VisionDistance)
            {
                // Si ambas condiciones se cumplen, el jugador es detectado
                detected = true;
            }
        }
    }

    // Método para dibujar en la escena (útil para depuración)
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
        Gizmos.DrawLine(Head.position, Head.position + p1);
        Gizmos.DrawLine(Head.position, Head.position + p2);
        // Dibujar una línea central hacia adelante desde la cabeza
        Gizmos.DrawRay(Head.position, Head.forward * VisionDistance);
    }

    // Método para calcular un punto en el espacio 3D dado un ángulo y una distancia
    Vector3 PointForAngle(float Angle, float Distance)
    {
        // Convertir el ángulo a radianes
        float rad = Angle * Mathf.Deg2Rad;
        // Calcular y devolver el punto usando seno y coseno
        return Head.TransformDirection(new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * Distance);
    }
}
