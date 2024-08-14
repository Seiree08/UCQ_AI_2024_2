using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : BaseState
{
    private enum AlertSubState
    {
        Stopped = 1,
        GoingToCheck = 2,
        ReturningToPosition = 3,
        MAX = 4
    }

    private AlertSubState _currentSubState = AlertSubState.Stopped;

    // GameObject que es el Player al cual nuestro patrullero debe detectar.
    public GameObject PlayerGameObject;

    // Referencia al Scriptable Object que contiene varios valores que describen c�mo se debe comportar este estado.
    private AlertStateScriptableObject _stateValues;

    private float _accumulatedTimeDetectingPlayerBeforeEnteringAttack;

    //  Cantidad de tiempo de la �ltima vez que detect� al jugador, es decir, en qu� momento en el tiempo se detect�.
    private float _lastTimePlayerSeen = 0;

    private Vector3 _lastKnownPlayerLocation = Vector3.zero;

    public AlertState(BaseFSM inBaseFSM) : base("Alert", inBaseFSM)
    {

    }

    public virtual void InitializeState(BaseFSM inFSMRef, GameObject inPlayerGameObject,
        AlertStateScriptableObject inStateValues)
    {
        Name = "AlertState";

        base.InitializeState(inFSMRef);

        _stateValues = inStateValues;
        PlayerGameObject = inPlayerGameObject;
    }


    public override void OnEnter()
    {
        base.OnEnter();
        // Obtenemos el momento (en tiempo de reloj) en que se entr� a este estado,
        // y se lo ponemos como la �ltima vez que vimos al player.
        _lastTimePlayerSeen = Time.realtimeSinceStartup;

        // reiniciamos las variables que se necesiten reiniciar cada que se entre a este estado.
        _accumulatedTimeDetectingPlayerBeforeEnteringAttack = 0.0f;
        _currentSubState = AlertSubState.Stopped; // Iniciamos en el sub-estado Stopped siempre.
    }

    // NOTA:
    // Por consistencia, si cambiamos de sub-estados, tambi�n vamos a llamar return, como si 
    // fueran estados normales y no sub-estados.

    // Update is called once per frame
    public override void OnUpdate()
    {
        // base.OnUpdate();
        bool DetectedPlayer = TargetIsInRange();

        // Primero que nada, actualizamos la "vista" de este estado.
        if (DetectedPlayer)
        {
            // si s� lo vimos, actualizamos la �ltima posici�n conocida del player.
            _lastKnownPlayerLocation = PlayerGameObject.transform.position;
        }

        // Cubrimos el comportamiento del Sub-estado de Stopped, que es cuando est� sin moverse tratando de ver 
        // al player durante m�s tiempo para ver si pasa al estado de ataque.
        if (_currentSubState == AlertSubState.Stopped)
        {
            // Aqu� tendr�amos la parte que ya conocemos sobre cambiar al estado de Ataque
            // Si s� estamos viendo al jugador, hacemos lo siguiente:
            if (DetectedPlayer)
            {
                // tenemos que estar en el sub-estado Stopped para seguir acumulando este tiempo.
                _accumulatedTimeDetectingPlayerBeforeEnteringAttack += Time.deltaTime;
                if (_accumulatedTimeDetectingPlayerBeforeEnteringAttack >= _stateValues.TimeSeeingInfiltratorBeforeEnteringAttack)
                {
                    Debug.Log("Ya vi suficiente al player, puedo pasar al estado de Ataque.");
                    // si el tiempo acumulado es mayor, pasar�amos a Attack.
                    return;
                }

                // entonces lo acabamos de ver, despu�s.
                // y guardamos el momento en que lo vimos, para despu�s poder checar si ya pasaron los X segundos
                // antes de ir a checar la �ltima posici�n conocida del player.
                _lastTimePlayerSeen = Time.realtimeSinceStartup;

                //if (_lastTimePlayerSeen == -1.0f)
                //{


                //    Debug.Log("Actualic� la �ltima vez que vi al player en Stopped.");
                //}
            }
            else
            {
                // Si no estamos haciendo cosas para pasar al estado de ataque, 
                // entonces, por nuestro dise�o, estar�amos haciendo cosas que nos devuelvan al estado de Patrullaje.
                float transcurredTime = Time.realtimeSinceStartup - _lastTimePlayerSeen;

                // Si despu�s de un cierto tiempo de la �ltima vez que detect� al jugador ya no lo ha visto.
                if (_stateValues.TimeSinceLastSeenTreshold < transcurredTime)
                {
                    Debug.Log("Ya no he visto al player por un rato, cambiando de Stopped a GoingToCheck.");

                    // entonces ponemos LastTimePlayerSeen en -1 (valor que nosotros definimos),
                    // para que sepamos que no es v�lido ahorita.
                    // _lastTimePlayerSeen = -1.0f;

                    // Y le decimos a este enemigo que vaya a la �ltima posici�n conocida del player,
                    // al pon�rsela al NavMeshAgent como su destination.
                    /* Recordatorio, tenemos que castearla al tipo espec�fico de FSM que es, para poder
                    acceder a las variables de dicho tipo espec�fico. */
                    ((EnemyFSM)FSMRef).NavMeshAgentRef.SetDestination(_lastKnownPlayerLocation);

                    // Despu�s, ponemos el sub-estado a GoingToCheck. 
                    _currentSubState = AlertSubState.GoingToCheck;
                    // Y finalmente hacemos el return porque cambiamos de Sub-estado.
                    return;
                }
            }
        }
        else if (_currentSubState == AlertSubState.GoingToCheck)
        {
            // Si vemos otra vez al jugador, inmediatamente pasamos al estado de Stopped.
            if (DetectedPlayer)
            {
                Debug.Log("Vi al player mientras estaba en Alert.GoingToCheck, por lo que vuelvo a Alert.Stopped desde 0.");

                // Si pas� esto, tenemos que hacer que se deje de mover. As� que le ponemos su propia posici�n como destination.
                ((EnemyFSM)FSMRef).NavMeshAgentRef.SetDestination(FSMRef.transform.position);

                // en el sub-estado de Stopped ya se encargar�n de poner el _lastTimePlayerSeen adecuadamente.
                _currentSubState = AlertSubState.Stopped;
                // salimos de este sub-estado.
                return;
            }

            // Si no lo hemos visto, entonces:
            // Tenemos que checar si ya llegamos a la posici�n deseada (que es la �ltima posici�n conocida)
            // Le damos rango de tolerancia a esta distancia entre nuestra posici�n y la �ltima posici�n conocida
            float dist = Vector3.Distance(FSMRef.transform.position, _lastKnownPlayerLocation);
            if (dist < _stateValues.DistanceToGoalTolerance)  // OJO: el NavMeshAgent ya tiene un valor para esto, f�jense que no se estorben.
            {
                Debug.Log("Ya llegu� a la �ltima posici�n conocida, ahora paso a Alert.ReturningToPosition.");
                // Entonces ya estamos lo suficientemente cerca,
                // y este enemigo puede empezar a regresar a su InitialPatrolPosition.
                // Le ponemos al NavMeshAgent que su "destination" es esa initial Patrol position.
                ((EnemyFSM)FSMRef).NavMeshAgentRef.SetDestination(((EnemyFSM)FSMRef).InitialPatrolPosition);
                _currentSubState = AlertSubState.ReturningToPosition;
                return;
            }
        }
        else if (_currentSubState == AlertSubState.ReturningToPosition)
        {
            // Seguir checando a ver si de camino a la posici�n inicial se detecta al player.
            // Si s� lo vemos, nos vamos al estado de stopped
            if (DetectedPlayer)
            {
                // Si pas� esto, tenemos que hacer que se deje de mover. As� que le ponemos su propia posici�n como destination.
                ((EnemyFSM)FSMRef).NavMeshAgentRef.SetDestination(FSMRef.transform.position);

                Debug.Log("Vi al player mientras estaba en Alert.ReturningToPosition, por lo que vuelvo a Alert.Stopped desde 0.");
                // en el sub-estado de Stopped ya se encargar�n de poner el _lastTimePlayerSeen adecuadamente.
                _currentSubState = AlertSubState.Stopped;
                // salimos de este sub-estado.
                return;
            }

            // Checamos si este enemigo ya lleg� a su posici�n inicial de patrullaje.
            if (Vector3.Distance(FSMRef.transform.position, ((EnemyFSM)FSMRef).InitialPatrolPosition) < _stateValues.DistanceToGoalTolerance)
            {
                Debug.Log("Ya llegu� a mi posici�n inicial, cambiar� al estado de Patrol.");
                // Si s�, puede pasar al estado de patrullaje.
                FSMRef.ChangeState(((EnemyFSM)FSMRef).PatrolStateRef);
                // Hacemos return porque estamos saliendo de este estado.
                return;
            }
            // Si no, pues seguir� movi�ndose hacia su posici�n inicial de patrullaje hasta llegar.
        }

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    private bool TargetIsInRange()
    {
        // Mandamos a llamar la funci�n de la FSM, y le pasamos los valores que este estado ya contiene.
        return ((EnemyFSM)FSMRef).TargetIsInRange(PlayerGameObject.transform.position, _stateValues.VisionDistance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Dibujo de una esfera en la �ltima posici�n conocida del player.
        Gizmos.DrawSphere(_lastKnownPlayerLocation, _stateValues.LastKnownPositionDebugSphereRadius);
    }
}