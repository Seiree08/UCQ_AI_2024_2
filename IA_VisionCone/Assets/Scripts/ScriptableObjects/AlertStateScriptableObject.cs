using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AlertStateScriptableObject", order = 1)]
public class AlertStateScriptableObject : ScriptableObject
{

    // Alert State
    public float VisionAngle = 60.0f;
    public float VisionDistance = 15.0f;
    // Cu?nto tiempo tiene que ver al Infiltrador para pasar al estado de Alerta.
    public float TimeSeeingInfiltratorBeforeEnteringAttack = 2.0f;

    // Cu?nto tiempo debe pasar entre la ?ltima vez que vio al jugador, para decir: ya toca ir a la ?ltima posici?n conocida.
    public float TimeSinceLastSeenTreshold = 2;

    // Rango de distancia respecto a su posici?n objetivo, en el que nuestro agente ya puede detenerse
    // la necesitamos porque el NavMesh no nos avisa directamente cuando ya acab?.
    public float DistanceToGoalTolerance = 1.0f;

    // Radio de la esfera del gizmo de la ?ltima posici?n conocida del player.
    public float LastKnownPositionDebugSphereRadius = 1.0f;

    // SE ACTUALIZA A TRAV?S DE LA FUNCI?N CheckFieldOfView DIRECTAMENTE.
    // la ?ltima posici?n conocida del sospechoso
    // Si nos ponemos elaborados, hasta podr?amos darle un margen de error
    //private Vector3 LastKnownPlayerLocation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}