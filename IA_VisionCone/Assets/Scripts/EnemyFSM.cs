using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : BaseFSM
{
    // Necesitamos que la m�quina contenga una referencia a cada estado que va a poseer.
    private PatrolState PatrolStateInstance;
    private AlertState AlertStateInstance;

    // Estos "getters" se usan para que los estados de esta FSM puedan LEER los estados pero no modificarlos.
    public PatrolState PatrolStateRef
    {
        get { return PatrolStateInstance; }
    }


    public AlertState AlertStateRef
    {
        get { return AlertStateInstance; }
    }

    // Tambi�n necesitamos que la m�quina contenga una referencia a los
    // ScriptableObjects de cada estado que posee.
    [SerializeField]
    private PatrolStateScriptableObject PatrolStateValues;

    [SerializeField]
    private AlertStateScriptableObject AlertStateValues;

    // Posici�n de patrullaje inicial a la cual volver� despu�s de Alert o Attack, seg�n corresponda.
    private Vector3 _initialPatrolPosition;
    public Vector3 InitialPatrolPosition
    {
        get { return _initialPatrolPosition; }
    }

    public GameObject PlayerGameObject;

    // NavMeshAgent del due�o de esta FSM. Se usa para decirle a qu� posici�n moverse.
    // Asignarlo (al GameObject que tenga este script de FSM) en el editor para poder obtenerlo en el start
    // y poder usarlo; si no, tronar�. 
    // Los estados pueden acceder a �l a trav�s de la m�quina de estados.
    private NavMeshAgent _navMeshAgentRef;
    public NavMeshAgent NavMeshAgentRef
    {
        get { return _navMeshAgentRef; }
    }

    // Al igual que con el _navMeshAgent, hay que signarlo al
    // GameObject que tenga este script de FSM en el editor para poder obtenerlo en el start
    private MeshRenderer _meshRendererRef;

    public MeshRenderer MeshRenderer
    {
        get { return _meshRendererRef; }
    }

    // Usaremos esta wall layer mask para hacer que el patrullero no pueda ver al jugador a trav�s de las paredes.
    // La inicializamos en el Start de nuestra FSM.
    // Para que funcione, tienen que ir a "Tags & Layers", agregar una nueva layer que se llame "Wall", y
    // asignarle dicha Layer a los gameObjects en su escena que sean paredes. Les recomiendo hacer un gameObject
    // vac�o que sea padre de las walls de su escenario, y hacer que ese padre tenga el Layer de wall, y aplicarle
    // el cambio de layer a todos los hijos (vean la Jerarqu�a de mi escena para que vean que todas mis walls
    // tienen el tag de Wall).
    private LayerMask _wallLayerMask;

    // private AttackState AttackStateInstance;

    // Start is called before the first frame update
    public override void Start()
    {
        // Aqu� inicializamos esto, que usaremos con un raycast m�s abajo.
        // _wallLayerMask = LayerMask.GetMask("Wall");

        if (!TryGetComponent<MeshRenderer>(out _meshRendererRef))
        {
            Debug.LogError("Invalid MeshRenderer in the FSM.");
            return;
        }

        // Le decimos que su posici�n inicial de patrullaje es aquella en la que estaba cuando se le dio play a la escena.
        _initialPatrolPosition = transform.position;

        PlayerGameObject = GameObject.FindGameObjectWithTag("Player");
        if (PlayerGameObject == null)
        {
            Debug.LogError("Invalid PlayerGameObject in the FSM.");
            return;
        }

        // Obtenemos el NavMeshAgent que est� asignado a este GameObject en el editor.
        // Al parecer, esto de TryGetComponent es la manera recomendada de checar si tiene un componente y,
        // si s� lo tiene, asignarlo (por eso usa la palabra "out" porque lo asigna en esa variable).
        if (!TryGetComponent<NavMeshAgent>(out _navMeshAgentRef))
        {
            Debug.LogError("Invalid NavMeshAgent in the FSM.");
            return;
        }

        // Antes de asignar el estado inicial, hay que crear los estados de esta FSM.


        // Recuerden que, como necesitamos que nuestros estados hereden de MonoBehavior para que puedan usar Corrutinas, 
        // entonces tenemos que a�adirlos con AddComponent, no podemos crearlos usando new como antes.
        PatrolStateInstance = this.AddComponent<PatrolState>();

        // Ahora nos basta con pasarle la referencia al Scriptable Object que tiene los valores deseados de dicho estado.
        PatrolStateInstance.InitializeState(this, PlayerGameObject, PatrolStateValues, InitialPatrolPosition);

        // Hacemos lo mismo con los otros estados seg�n corresponda.
        AlertStateInstance = this.AddComponent<AlertState>();
        AlertStateInstance.InitializeState(this, PlayerGameObject, AlertStateValues);

        // Finalmente, ya que tenemos completo el setup de la FSM, la iniciamos con la funci�n Start de su clase padre.
        base.Start();
    }


    // Tenemos que sobreescribir la funci�n de GetInitialState para que no sea null.
    public override BaseState GetInitialState()
    {
        // Aqu� el estado inicial deber�a ser Patrol State, ya que lo implementemos.
        return PatrolStateInstance;
    }

    public bool TargetIsInRange(Vector3 targetPosition, float visionRange)
    {
        // Diferencia entre mi posici�n y la posici�n de mi objetivo.
        // Queremos la magnitud de esa distancia.
        float distance = (targetPosition - transform.position).magnitude;

        // Si la distancia entre la posici�n de mi objetivo y mi posici�n es mayor que mi rango de visi�n, entonces...
        if (distance > visionRange)
        {
            // le pongo un color de material para cuando No lo vio.
            _meshRendererRef.material.color = new Color(1, 0, 0, 1);

            // Entonces no podemos ver a ese objetivo.
            return false;
        }

        // le pongo un color de material para cuando s� lo vio.
        _meshRendererRef.material.color = new Color(1, 0, 1, 1);

        // Entonces s� lo podr�a ver.
        return true;
    }
}