using System.Collections.Generic;
using UnityEngine;

public class EnergyFlowManager : MonoBehaviour
{
    [Header("Elementos del puzzle")]
    [SerializeField] private List<RootSegment> segments = new List<RootSegment>();
    [SerializeField] private List<EnergyNode> nodes = new List<EnergyNode>();

    [Header("Grilla lógica")]
    [Tooltip("Tamaño de cada celda de la grilla, debería coincidir con la distancia entre segmentos.")]
    [SerializeField] private float cellSize = 1.5f;

    [Header("Feedback de Árbol")]
    [SerializeField] private TreeEnergyController treeController;
    [SerializeField] private GameObject pathBlocked;     // raíces cerrando el paso
    [SerializeField] private GameObject pathOpened;      // raíces abiertas / pasarela

    private EnergyNode sourceNode;
    private List<EnergyNode> targetNodes = new List<EnergyNode>();

    // Para debug/estado
    public bool isFlowComplete { get; private set; }

    // Representa una celda en la grilla
    private class Cell
    {
        public Vector2Int gridPos;
        public RootSegment segment;
        public EnergyNode node;
    }

    private Dictionary<Vector2Int, Cell> grid = new Dictionary<Vector2Int, Cell>();

    private void Awake()
    {
        // Si no se cargaron manualmente, buscar en escena
        if (segments.Count == 0)
            segments.AddRange(Object.FindObjectsByType<RootSegment>(FindObjectsSortMode.None));

        if (nodes.Count == 0)
            nodes.AddRange(Object.FindObjectsByType<EnergyNode>(FindObjectsSortMode.None));

        BuildGrid();
        CacheSourceAndTargets();
        SubscribeToSegments();
        RecalculateFlow();
    }

    private void OnDestroy()
    {
        UnsubscribeFromSegments();
    }

    private void BuildGrid()
    {
        grid.Clear();

        // Celdas para segmentos
        foreach (var seg in segments)
        {
            Vector2Int pos = WorldToGrid(seg.transform.position);

            if (!grid.TryGetValue(pos, out var cell))
            {
                cell = new Cell { gridPos = pos };
                grid.Add(pos, cell);
            }

            cell.segment = seg;
        }

        // Celdas para nodos
        foreach (var node in nodes)
        {
            Vector2Int pos = WorldToGrid(node.transform.position);

            if (!grid.TryGetValue(pos, out var cell))
            {
                cell = new Cell { gridPos = pos };
                grid.Add(pos, cell);
            }

            cell.node = node;
        }
    }

    private void CacheSourceAndTargets()
    {
        sourceNode = null;
        targetNodes.Clear();

        foreach (var n in nodes)
        {
            if (n.isSource)
                sourceNode = n;

            if (n.isTarget)
                targetNodes.Add(n);
        }

        if (sourceNode == null)
        {
            Debug.LogWarning($"[{name}] No se encontró EnergyNode con isSource = true");
        }

        if (targetNodes.Count == 0)
        {
            Debug.LogWarning($"[{name}] No se encontraron EnergyNodes con isTarget = true");
        }
    }

    private void SubscribeToSegments()
    {
        foreach (var seg in segments)
        {
            seg.OnSegmentRotated += OnSegmentChanged;
        }
    }

    private void UnsubscribeFromSegments()
    {
        foreach (var seg in segments)
        {
            seg.OnSegmentRotated -= OnSegmentChanged;
        }
    }

    private void OnSegmentChanged()
    {
        RecalculateFlow();
    }

    /// <summary>
    /// Recalcula si existe un camino continuo de energía del source al target.
    /// </summary>
    public void RecalculateFlow()
    {
        if (sourceNode == null || targetNodes.Count == 0)
        {
            isFlowComplete = false;
            ApplyPoweredSegments(null); // apaga todos
            return;
        }

        var visited = new HashSet<Cell>();
        var queue = new Queue<Cell>();
        var cameFrom = new Dictionary<Cell, Cell>(); // <actual, from>

        Cell sourceCell = GetCellAtPosition(WorldToGrid(sourceNode.transform.position));
        if (sourceCell == null)
        {
            Debug.LogWarning("SourceNode no está en la grilla.");
            isFlowComplete = false;
            ApplyPoweredSegments(null);
            return;
        }

        queue.Enqueue(sourceCell);
        visited.Add(sourceCell);

        Cell targetCellFound = null;

        while (queue.Count > 0)
        {
            Cell current = queue.Dequeue();

            // ¿Esta celda contiene un target?
            if (current.node != null && current.node.isTarget)
            {
                targetCellFound = current;
                break;
            }

            // Obtener conexiones de esta celda (segmento o nodo)
            List<RootSegment.Direction> currentConnections = GetConnectionsForCell(current);

            foreach (var dir in currentConnections)
            {
                Vector2Int neighborPos = current.gridPos + DirectionToOffset(dir);
                Cell neighbor = GetCellAtPosition(neighborPos);

                if (neighbor == null || visited.Contains(neighbor))
                    continue;

                // El vecino debe tener una conexión en dirección opuesta
                List<RootSegment.Direction> neighborConnections = GetConnectionsForCell(neighbor);
                RootSegment.Direction opposite = GetOppositeDirection(dir);

                if (neighborConnections.Contains(opposite))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current; // guardamos de dónde vino
                }
            }
        }

        if (targetCellFound != null)
        {
            isFlowComplete = true;
            Debug.Log($"[{name}] Flujo completo: TRUE");

            // Reconstruir el camino concreto Source -> Target
            List<Cell> pathCells = ReconstructPath(sourceCell, targetCellFound, cameFrom);

            // Aplicar power solo a los segmentos que están en el camino
            ApplyPoweredSegments(pathCells);
            OnFlowCompleted();
        }
        else
        {
            isFlowComplete = false;
            Debug.Log($"[{name}] Flujo completo: FALSE");

            // No hay camino, nadie está energizado
            ApplyPoweredSegments(null);
            OnFlowBroken();
        }
    }

    private List<Cell> ReconstructPath(Cell start, Cell goal, Dictionary<Cell, Cell> cameFrom)
    {
        List<Cell> path = new List<Cell>();
        Cell current = goal;

        path.Add(current);

        while (current != start)
        {
            if (!cameFrom.TryGetValue(current, out var prev))
            {
                // Algo se rompió en el diccionario, devolvemos lo que haya
                break;
            }

            current = prev;
            path.Add(current);
        }

        path.Reverse(); // ahora es start -> goal
        foreach(Cell cell in path)
        {
            Debug.Log(cell.gridPos);
        }
        return path;
    }

    private void ApplyPoweredSegments(List<Cell> pathCells)
    {
        // Primero, apagar todos
        foreach (var seg in segments)
        {
            if (seg != null)
                seg.SetPowered(false);
        }

        if (pathCells == null) return;

        // Encender solo los segmentos que están en el camino
        foreach (var cell in pathCells)
        {
            if (cell.segment != null)
            {
                cell.segment.SetPowered(true);
            }
        }
    }


    private Cell GetCellAtPosition(Vector2Int pos)
    {
        grid.TryGetValue(pos, out var cell);
        return cell;
    }

    private Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int z = Mathf.RoundToInt(worldPos.z / cellSize);
        return new Vector2Int(x, z);
    }

    private Vector2Int DirectionToOffset(RootSegment.Direction dir)
    {
        switch (dir)
        {
            case RootSegment.Direction.North: return new Vector2Int(0, 1);
            case RootSegment.Direction.East: return new Vector2Int(1, 0);
            case RootSegment.Direction.South: return new Vector2Int(0, -1);
            case RootSegment.Direction.West: return new Vector2Int(-1, 0);
        }

        return Vector2Int.zero;
    }

    private RootSegment.Direction GetOppositeDirection(RootSegment.Direction dir)
    {
        switch (dir)
        {
            case RootSegment.Direction.North: return RootSegment.Direction.South;
            case RootSegment.Direction.East: return RootSegment.Direction.West;
            case RootSegment.Direction.South: return RootSegment.Direction.North;
            case RootSegment.Direction.West: return RootSegment.Direction.East;
        }

        return dir;
    }

    /// <summary>
    /// Devuelve las conexiones de una celda, ya sea de su segmento o de su nodo.
    /// </summary>
    private List<RootSegment.Direction> GetConnectionsForCell(Cell cell)
    {
        // Prioridad: si hay segmento, usamos sus conexiones; si no, las del nodo.
        if (cell.segment != null)
        {
            return cell.segment.GetCurrentConnections();
        }
        else if (cell.node != null)
        {
            return cell.node.GetConnections();
        }

        return new List<RootSegment.Direction>();
    }

    private void OnFlowCompleted()
    {
        if (treeController != null)
        {
            treeController.SetState(true);
        }
        // TODO: encender árbol, partículas, sonido, etc.
        // Podés referenciar acá al árbol visual del nivel.

        if (pathBlocked != null) pathBlocked.SetActive(false);
        if (pathOpened != null) pathOpened.SetActive(true);

        foreach(RootSegment segment in segments)
        {
            segment.gameObject.layer = LayerMask.NameToLayer("IgnoreInteractable");
        }
    }

    private void OnFlowBroken()
    {
        if (treeController != null)
        {
            treeController.SetState(false);
        }
        // TODO: apagar efectos, etc. si querés feedback.
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));

        // Mostrar grilla calculada en editor (solo si ya hay elementos)
        if (segments != null && segments.Count > 0)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
            foreach (var seg in segments)
            {
                Vector2Int pos = WorldToGrid(seg.transform.position);
                Vector3 world = new Vector3(pos.x * cellSize, seg.transform.position.y, pos.y * cellSize);
                Gizmos.DrawWireCube(world, new Vector3(cellSize * 0.9f, 0.1f, cellSize * 0.9f));
            }
        }
    }
#endif
}
