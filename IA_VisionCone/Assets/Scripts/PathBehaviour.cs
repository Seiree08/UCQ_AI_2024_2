using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Nodo
{
    public string Id;
    public Vector3 Position;
    public Nodo Parent;

    public Nodo(string id, Vector3 position)
    {
        this.Id = id;
        this.Position = position;
    }
}

[System.Serializable]
public class Edge
{
    public Nodo A;
    public Nodo B;

    public Edge(Nodo a, Nodo b)
    {
        this.A = a;
        this.B = b;
    }
}

public class PathBehaviour : MonoBehaviour
{
    public List<Edge> edges = new List<Edge>();
    public List<Nodo> nodes = new List<Nodo>();

    public GameObject nodePrefab;
    public GameObject edgePrefab;
    public GameObject movingObjectPrefab;
    public GameObject nodeTextPrefab;

    [Header("Nodos de Inicio y Objetivo")]
    public string startNodeId;
    public string goalNodeId;

    private Dictionary<string, GameObject> nodeObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> nodeTexts = new Dictionary<string, GameObject>();
    private GameObject movingObject;

    private bool isSearching = false;
    private bool isMoving = false;
    private List<Nodo> openList;
    private List<Nodo> closedList;
    private Nodo currentNode;
    private Nodo goalNode;
    private List<Nodo> path = new List<Nodo>();
    private float moveSpeed = 5.0f;
    private float proximityThreshold = 0.1f;

    [Header("Configuraciones de Búsqueda")]
    public float searchDelay = 1.0f;
    private float searchTimer = 0.0f;

    public int MaxNodesToVisit = 10;

    void Start()
    {
        CreateGraph();

        Nodo startNode = nodes.Find(n => n.Id == startNodeId);
        goalNode = nodes.Find(n => n.Id == goalNodeId);

        if (startNode != null && goalNode != null)
        {
            // Instanciar el objeto en movimiento en la posición inicial del nodo de inicio
            if (movingObject == null)
            {
                movingObject = Instantiate(movingObjectPrefab, startNode.Position, Quaternion.identity);
            }

            // Inicializar la búsqueda BFS
            InitializeBFS(startNode, goalNode);
        }
        else
        {
            Debug.LogError("Nodo inicial o nodo objetivo no encontrados.");
        }
    }

    void Update()
    {
        if (isSearching)
        {
            searchTimer += Time.deltaTime;
            if (searchTimer >= searchDelay)
            {
                searchTimer = 0.0f;
                PerformBFS();
            }
        }

        if (isMoving && movingObject != null)
        {
            MoveObjectAlongPath();
        }
    }

    void CreateGraph()
    {
        Vector3[] positions = new Vector3[]
        {
            new Vector3(0, 0, 4),
            new Vector3(-2, 0, 2),
            new Vector3(-4, 0, 0),
            new Vector3(-2, 0, 0),
            new Vector3(2, 0, 2),
            new Vector3(0, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(4, 0, 0)
        };

        List<string> ids = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };

        nodes.Clear();
        edges.Clear();

        for (int i = 0; i < positions.Length; i++)
        {
            Nodo node = new Nodo(ids[i], positions[i]);
            nodes.Add(node);
        }

        edges.Add(new Edge(nodes[0], nodes[1]));
        edges.Add(new Edge(nodes[0], nodes[4]));
        edges.Add(new Edge(nodes[1], nodes[2]));
        edges.Add(new Edge(nodes[1], nodes[3]));
        edges.Add(new Edge(nodes[4], nodes[5]));
        edges.Add(new Edge(nodes[4], nodes[6]));
        edges.Add(new Edge(nodes[4], nodes[7]));

        CreateVisualNodesAndEdges();
    }

    void CreateVisualNodesAndEdges()
    {
        ShuffleNodeIds();

        foreach (Nodo node in nodes)
        {
            GameObject nodeGO = Instantiate(nodePrefab, node.Position, Quaternion.identity);
            nodeGO.name = node.Id;
            nodeObjects[node.Id] = nodeGO;

            GameObject textGO = Instantiate(nodeTextPrefab, node.Position + new Vector3(0, 1, 0), Quaternion.Euler(90, 0, 0));
            textGO.GetComponent<TextMeshPro>().text = node.Id;
            textGO.transform.SetParent(nodeGO.transform);
            nodeTexts[node.Id] = textGO;
        }

        foreach (Edge edge in edges)
        {
            GameObject edgeGO = Instantiate(edgePrefab);
            LineRenderer lr = edgeGO.GetComponent<LineRenderer>();
            lr.SetPosition(0, edge.A.Position);
            lr.SetPosition(1, edge.B.Position);
        }
    }

    void ShuffleNodeIds()
    {
        List<string> ids = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H" };
        for (int i = 0; i < nodes.Count; i++)
        {
            int randomIndex = Random.Range(0, ids.Count);
            nodes[i].Id = ids[randomIndex];
            ids.RemoveAt(randomIndex);
        }
    }

    void InitializeBFS(Nodo startNode, Nodo goalNode)
    {
        startNode.Parent = null;
        openList = new List<Nodo> { startNode };
        closedList = new List<Nodo>();
        this.goalNode = goalNode;
        isSearching = true;
        isMoving = false;

        // Mover el objeto a la posición inicial
        MoveObjectToNode(startNode);
    }

    void PerformBFS()
    {
        if (openList.Count > 0)
        {
            currentNode = openList[0];
            openList.RemoveAt(0);
            closedList.Add(currentNode);

            if (nodeObjects.TryGetValue(currentNode.Id, out GameObject currentGO))
            {
                currentGO.GetComponent<Renderer>().material.color = Color.red;
            }

            if (currentNode == goalNode)
            {
                path.Clear();
                Nodo pathNode = goalNode;
                while (pathNode != null)
                {
                    path.Add(pathNode);
                    pathNode = pathNode.Parent;
                }
                path.Reverse();

                foreach (Nodo nodo in path)
                {
                    if (nodeObjects.TryGetValue(nodo.Id, out GameObject pathGO))
                    {
                        pathGO.GetComponent<Renderer>().material.color = Color.green;
                    }
                }

                isMoving = true;
                isSearching = false;
                return;
            }

            List<Nodo> neighbors = FindNeighbors(currentNode);
            foreach (Nodo neighbor in neighbors)
            {
                if (openList.Contains(neighbor) || closedList.Contains(neighbor))
                    continue;

                neighbor.Parent = currentNode;
                openList.Add(neighbor);
            }
        }
        else
        {
            if (closedList.Count >= MaxNodesToVisit && !closedList.Contains(goalNode))
            {
                Debug.Log("Ruta no encontrada.");
            }
            isSearching = false;
        }
    }

    List<Nodo> FindNeighbors(Nodo node)
    {
        List<Nodo> neighbors = new List<Nodo>();
        foreach (Edge edge in edges)
        {
            if (edge.A == node)
            {
                neighbors.Add(edge.B);
            }
            if (edge.B == node)
            {
                neighbors.Add(edge.A);
            }
        }
        return neighbors;
    }

    void MoveObjectToNode(Nodo node)
    {
        if (movingObject != null)
        {
            movingObject.transform.position = node.Position;
        }
    }

    void MoveObjectAlongPath()
    {
        if (movingObject != null && path.Count > 0)
        {
            Vector3 targetPosition = path[0].Position;
            movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(movingObject.transform.position, targetPosition) < proximityThreshold)
            {
                path.RemoveAt(0);
                if (path.Count == 0)
                {
                    isMoving = false;
                }
            }
        }
    }
}
