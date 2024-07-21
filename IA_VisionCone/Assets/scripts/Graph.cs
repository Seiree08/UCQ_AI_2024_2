using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clase que representa un nodo en el grafo
public class Node
{
    public string Id;        // Identificador del nodo
    public Vector3 Position; // Posición en el espacio 3D
    public Node Parent;      // Nodo padre en el camino de búsqueda

    public Node(string id, Vector3 position)
    {
        this.Id = id;
        this.Position = position;
    }
}

// Clase que representa una arista entre dos nodos
public class Lado
{
    public Node A; // Nodo A de la arista
    public Node B; // Nodo B de la arista

    public Lado(Node a, Node b)
    {
        this.A = a;
        this.B = b;
    }
}

// Clase que maneja el grafo y la búsqueda
public class Graph : MonoBehaviour
{
    public List<Lado> edges = new List<Lado>(); // Lista de aristas en el grafo
    public List<Node> nodes = new List<Node>(); // Lista de nodos en el grafo

    // Prefabs para los nodos y aristas en la escena
    public GameObject nodePrefab;
    public GameObject edgePrefab;

    private Dictionary<string, GameObject> nodeObjects = new Dictionary<string, GameObject>(); // Mapa para almacenar objetos de nodo

    // Variables para la búsqueda BFS (Breadth-First Search)
    private bool isSearching = false; // Estado de la búsqueda
    private List<Node> openList; // Lista de nodos a explorar
    private List<Node> closedList; // Lista de nodos ya explorados
    private Node goalNode; // Nodo objetivo de la búsqueda
    private Node currentNode; // Nodo actualmente en exploración
    private int maxNodesToVisit; // Cantidad máxima de nodos a visitar

    // Variables para controlar la velocidad de la animación de búsqueda
    public float searchSpeed = 1.0f; // Velocidad de la búsqueda (mayor valor es más lento)
    private float searchTimer = 0.0f; // Temporizador para la búsqueda

    // Asignar la cantidad máxima de nodos a visitar desde el Inspector
    public int MaxNodesToVisit = 10;

    // Start se llama al iniciar la escena
    void Start()
    {
        // Crear nodos con posiciones actualizadas
        Node a = new Node("a", new Vector3(0, 4, 0));
        Node b = new Node("b", new Vector3(-2, 2, 0));
        Node c = new Node("c", new Vector3(-4, 0, 0));
        Node d = new Node("d", new Vector3(-2, 0, 0));
        Node e = new Node("e", new Vector3(2, 2, 0));
        Node f = new Node("f", new Vector3(0, 0, 0));
        Node g = new Node("g", new Vector3(2, 0, 0));
        Node h = new Node("h", new Vector3(4, 0, 0));

        // Limpiar listas de nodos y aristas
        nodes.Clear();
        edges.Clear();

        // Añadir nodos al grafo
        nodes.Add(a);
        nodes.Add(b);
        nodes.Add(c);
        nodes.Add(d);
        nodes.Add(e);
        nodes.Add(f);
        nodes.Add(g);
        nodes.Add(h);

        // Añadir aristas al grafo según la configuración
        edges.Add(new Lado(a, b));
        edges.Add(new Lado(a, e));
        edges.Add(new Lado(b, c));
        edges.Add(new Lado(b, d));
        edges.Add(new Lado(e, f));
        edges.Add(new Lado(e, g));
        edges.Add(new Lado(e, h));

        // Crear representaciones visuales para nodos y aristas
        CreateVisualNodesAndEdges();

        // Inicializar BFS desde "H" con destino a "D"
        InitializeBFS(h, d);
    }

    // Crear representaciones visuales para nodos y aristas
    void CreateVisualNodesAndEdges()
    {
        // Instanciar nodos en la escena
        foreach (Node node in nodes)
        {
            GameObject nodeGO = Instantiate(nodePrefab, node.Position, Quaternion.identity);
            nodeGO.name = node.Id;
            nodeObjects[node.Id] = nodeGO;
        }

        // Instanciar aristas en la escena
        foreach (Lado edge in edges)
        {
            GameObject edgeGO = Instantiate(edgePrefab);
            LineRenderer lr = edgeGO.GetComponent<LineRenderer>();
            lr.SetPosition(0, edge.A.Position);
            lr.SetPosition(1, edge.B.Position);
        }
    }

    // Inicializar la búsqueda BFS
    void InitializeBFS(Node startNode, Node goalNode)
    {
        startNode.Parent = null; // El nodo inicial no tiene padre

        openList = new List<Node>(); // Lista de nodos a explorar
        openList.Add(startNode); // Añadir nodo inicial a la lista de nodos a explorar
        closedList = new List<Node>(); // Lista de nodos ya explorados

        this.goalNode = goalNode; // Establecer nodo objetivo
        this.currentNode = null; // Nodo actual en exploración

        isSearching = true; // Iniciar búsqueda
    }

    // Realizar la búsqueda BFS
    void PerformBFS()
    {
        if (openList.Count > 0 && closedList.Count < MaxNodesToVisit)
        {
            currentNode = openList[0]; // Obtener el primer nodo de la lista abierta
            openList.RemoveAt(0); // Eliminar el nodo de la lista abierta
            closedList.Add(currentNode); // Añadir nodo actual a la lista cerrada

            Debug.Log("Visitando nodo: " + currentNode.Id); // Imprimir el nodo visitado
            nodeObjects[currentNode.Id].GetComponent<Renderer>().material.color = Color.green; // Cambiar color del nodo actual a rojo

            // Si el nodo actual es el nodo objetivo
            if (currentNode == goalNode)
            {
                // Retroceder desde el nodo objetivo para mostrar el camino
                while (currentNode != null)
                {
                    Debug.Log("El nodo: " + currentNode.Id + " fue parte del camino a la meta.");
                    nodeObjects[currentNode.Id].GetComponent<Renderer>().material.color = Color.cyan; // Cambiar color del nodo a verde
                    currentNode = currentNode.Parent; // Retroceder al nodo padre
                }
                isSearching = false; // Detener búsqueda
                return;
            }

            // Obtener vecinos del nodo actual
            List<Node> neighbors = FindNeighbors(currentNode);
            foreach (Node neighbor in neighbors)
            {
                // Si el vecino ya está en la lista abierta o cerrada, saltar
                if (openList.Contains(neighbor) || closedList.Contains(neighbor))
                    continue;

                neighbor.Parent = currentNode; // Establecer nodo actual como padre del vecino
                openList.Add(neighbor); // Añadir vecino a la lista abierta

                // Crear una línea visual entre el nodo actual y el vecino
                GameObject pathGO = new GameObject();
                LineRenderer lr = pathGO.AddComponent<LineRenderer>();
                lr.SetPosition(0, currentNode.Position);
                lr.SetPosition(1, neighbor.Position);
                lr.startWidth = 0.05f;
                lr.endWidth = 0.05f;
                lr.material = new Material(Shader.Find("Sprites/Default")) { color = Color.white}; // Establecer color de la línea
            }
        }
        else
        {
            // Si no se encontró un camino o se alcanzó el número máximo de nodos
            if (closedList.Count >= MaxNodesToVisit && !closedList.Contains(goalNode))
            {
                Debug.Log("Ruta no encontrada."); // Imprimir mensaje si no se encuentra ruta
            }
            isSearching = false; // Detener búsqueda
        }
    }

    // Encontrar los vecinos de un nodo
    public List<Node> FindNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        foreach (Lado edge in edges)
        {
            if (edge.A == node)
            {
                neighbors.Add(edge.B); // Añadir nodo B si el nodo A es el nodo dado
            }
            if (edge.B == node)
            {
                neighbors.Add(edge.A); // Añadir nodo A si el nodo B es el nodo dado
            }
        }

        return neighbors; // Devolver lista de vecinos
    }

    // Update se llama una vez por frame
    void Update()
    {
        if (isSearching)
        {
            searchTimer += Time.deltaTime; // Incrementar temporizador
            if (searchTimer >= searchSpeed)
            {
                searchTimer = 0.0f; // Reiniciar temporizador
                PerformBFS(); // Realizar búsqueda BFS
            }
        }
    }
}
