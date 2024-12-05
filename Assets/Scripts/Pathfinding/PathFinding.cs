using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    public static PathFinding Instance { get; private set; }
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TileNode> tileNodes = new Dictionary<Vector3Int, TileNode>();

    private void Awake() { 
        Instance = this;
        InitializeTileNodes();
    }

    private void InitializeTileNodes() {
        // First, create all nodes
        foreach (Vector3Int position in tilemap.cellBounds.allPositionsWithin) {
            if (tilemap.HasTile(position)) {
                TileNode node = new TileNode {
                    tilemap = tilemap,
                    position = position,
                    cost = float.MaxValue,
                    heuristic = float.MaxValue,
                    previous = null
                };
                tileNodes[position] = node;
            }
        }
        
        // Then, set neighbors after all nodes are created
        foreach (var node in tileNodes.Values) {
            SetNodeNeighbours(node);
        }
    }

    public TileNode FindNodeCloseToPosition(Vector3 position){
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        var node = tileNodes.ContainsKey(cellPosition) ? tileNodes[cellPosition] : null;
        return node;
    }

    public TileNode FindNodeAtMinimumDistance(Vector3 position, float distance){
        return tileNodes.Values
            .Where(node => Vector3.Distance(node.GetPosition(), position) <= distance)
            .OrderBy(node => Vector3.Distance(node.GetPosition(), position))
            .FirstOrDefault();
    }

    public TileNode FindNodeCloseToPositionInDirection(Vector3 position, Vector3 direction) {
        direction.Normalize();
        const float MAX_ANGLE = 5f;
        
        var result = tileNodes
            .Where(node => {
                Vector3 nodeDirection = (node.Value.GetPosition() - position).normalized;
                float angle = Vector3.Angle(direction, nodeDirection);
                return angle <= MAX_ANGLE;
            })
            .OrderBy(node => Vector3.Distance(node.Value.GetPosition(), position))
            .FirstOrDefault();
        
        return result.Value != null ? result.Value : null;
    }

    public TileNode GetRandomNode(){
        var node = tileNodes.Values.ElementAt(Random.Range(0, tileNodes.Count));
        return node;
    }

    public List<TileNode> GetRandomPath(TileNode start = null, TileNode end = null){
        return FindPath(start ?? GetRandomNode(), end ?? GetRandomNode());
    }


    public List<TileNode> FindPath(TileNode start = null, TileNode end = null){
        ResetNodes();
        TileNode startNode = start;
        TileNode endNode = end;

        if (startNode == null || endNode == null) {
            Debug.LogError("Start or end node is null");
             return null;
        }

        startNode.cost = 0;
        startNode.heuristic = CalcUtils.DistanceToTarget(startNode.GetPosition(), endNode.GetPosition());

        List<TileNode> openList = new List<TileNode> { startNode };
        HashSet<TileNode> closedList = new HashSet<TileNode>();

        while(openList.Count > 0) {
            openList = openList.OrderBy(node => node.heuristic).ToList();
            TileNode current = openList[0];
            openList.RemoveAt(0);

            if(current == endNode) {
                return ReconstructPath(current);
            }

            foreach (TileNode.Neighbour neighbour in current.neighbours) {
                if(closedList.Contains(neighbour.node)) {
                    continue;
                }

                float tentativeCost = current.cost + neighbour.cost;
                if(tentativeCost < neighbour.node.cost) {
                    neighbour.node.cost = tentativeCost;
                    neighbour.node.heuristic = tentativeCost + CalcUtils.DistanceToTarget(neighbour.node.GetPosition(), endNode.GetPosition());
                    neighbour.node.previous = current;

                    if(!openList.Contains(neighbour.node)) {
                        openList.Add(neighbour.node);
                    }
                }
            }
            closedList.Add(current);
        }

        return null;
    }

    private List<TileNode> ReconstructPath(TileNode endNode){
        List<TileNode> path = new List<TileNode>();
        TileNode current = endNode;
        while(current != null){
            if (current.previous != null) {
                Debug.DrawLine(current.GetPosition(), current.previous.GetPosition(), Color.red, 5f);
            }
            path.Add(current);
            current = current.previous;
        }
        path.Reverse();
        return path;
        // Debug.Log("Path: " + string.Join(" -> ", path.Select(node => node.name)));
    } 

    private void ResetNodes(){
        foreach (var node in tileNodes.Values) {
            node.cost = float.MaxValue;
            node.heuristic = float.MaxValue;
            node.previous = null;
        }
    }

    private void SetNodeNeighbours(TileNode node){

        var upNeighbour = tileNodes.ContainsKey(node.position + Vector3Int.up) ? tileNodes[node.position + Vector3Int.up] : null;
        var downNeighbour = tileNodes.ContainsKey(node.position + Vector3Int.down) ? tileNodes[node.position + Vector3Int.down] : null;
        var leftNeighbour = tileNodes.ContainsKey(node.position + Vector3Int.left) ? tileNodes[node.position + Vector3Int.left] : null;
        var rightNeighbour = tileNodes.ContainsKey(node.position + Vector3Int.right) ? tileNodes[node.position + Vector3Int.right] : null;

        if (upNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = upNeighbour, cost = 1 });
        }
        if (downNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = downNeighbour, cost = 1 });
        }
        if (leftNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = leftNeighbour, cost = 1 });
        }
        if (rightNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = rightNeighbour, cost = 1 });
        }

        // Check up right
        var upRight = node.position + Vector3Int.up + Vector3Int.right;
        if (tileNodes.ContainsKey(upRight) && upNeighbour != null && rightNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = tileNodes[upRight], cost = 1 });
        }

        // Check down right
        var downRight = node.position + Vector3Int.down + Vector3Int.right;
        if (tileNodes.ContainsKey(downRight) && downNeighbour != null && rightNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = tileNodes[downRight], cost = 1 });
        }

        // Check up left
        var upLeft = node.position + Vector3Int.up + Vector3Int.left;
        if (tileNodes.ContainsKey(upLeft) && upNeighbour != null && leftNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = tileNodes[upLeft], cost = 1 });
        }

        // Check down left
        var downLeft = node.position + Vector3Int.down + Vector3Int.left;
        if (tileNodes.ContainsKey(downLeft) && downNeighbour != null && leftNeighbour != null) {
            node.neighbours.Add(new TileNode.Neighbour { node = tileNodes[downLeft], cost = 1 });
        }
    }
}

public class TileNode
{
    public Tilemap tilemap;
    public Vector3Int position;
    public float cost;
    public float heuristic;
    public TileNode previous;
    public List<Neighbour> neighbours = new List<Neighbour>();

    public Vector3 GetPosition(){
        return tilemap.CellToWorld(position) + new Vector3(0.5f, 0.5f, 0);
    }

    public class Neighbour
    {
        public TileNode node;
        public float cost;
    }
}