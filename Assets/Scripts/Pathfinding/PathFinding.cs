using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Burst;
using Sirenix.OdinInspector;
using System.IO;
using System.Runtime.ExceptionServices;

public  class PathFinding : MonoBehaviour
{
    public static PathFinding Instance { get; private set; }
    public PathNode startNode;
    public PathNode endNode;
    private List<PathNode> graph = new List<PathNode>();
    private PathNode[] nodes;

    private void Awake() { 
        Instance = this;
        nodes = FindObjectsOfType<PathNode>();

       fillGraph();
    }

    public PathNode FindNodeCloseToPosition(Vector3 position){
        return graph.OrderBy(node => Vector3.Distance(node.transform.position, position)).FirstOrDefault();
    }

    public PathNode FindNodeCloseToPositionInDirection(Vector3 position, Vector3 direction){
        direction.Normalize();
        return graph
            .Where(node => Vector3.Dot(node.transform.position - position, direction) > 0)
            .OrderBy(node => Vector3.Distance(node.transform.position, position))
            .FirstOrDefault();
    }

    public PathNode GetRandomNode(){
        return graph[Random.Range(0, graph.Count)];
    }

    public List<PathNode> GetRandomPath(PathNode start = null, PathNode end = null){
        return FindPath(start ?? GetRandomNode(), end ?? GetRandomNode());
    }


    public List<PathNode> FindPath(PathNode start = null, PathNode end = null){
        ResetNodes();
        startNode = start;
        endNode = end;

        startNode.cost = 0;
        startNode.heuristic = CalcUtils.DistanceToTarget(startNode.transform, endNode.transform);

        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        while(openList.Count > 0){
            openList = openList.OrderBy(node => node.heuristic).ToList();
            PathNode current = openList[0];
            openList.RemoveAt(0);

            if(current == endNode){
                return ReconstructPath(current);
            }

            foreach (PathNode.Neighbour neighbour in current.neighbours)
            {
                if(closedList.Contains(neighbour.node)){
                    continue;
                }

                float tentativeCost = current.cost + 1;
                if(tentativeCost < neighbour.node.cost){
                    neighbour.node.cost = tentativeCost;
                    neighbour.node.heuristic = tentativeCost + CalcUtils.DistanceToTarget(neighbour.node.transform, endNode.transform);
                    neighbour.node.previous = current;

                    if(!openList.Contains(neighbour.node)){
                        openList.Add(neighbour.node);
                    }
                }
            }
            closedList.Add(current);
        }
        return null;
    }

    private List<PathNode> ReconstructPath(PathNode endNode){
        List<PathNode> path = new List<PathNode>();
        PathNode current = endNode;
        while(current != null){
            if (current.previous != null) {
                Debug.DrawLine(current.transform.position, current.previous.transform.position, Color.red, 5f);
            }
            path.Add(current);
            current = current.previous;
        }
        path.Reverse();
        return path;
        // Debug.Log("Path: " + string.Join(" -> ", path.Select(node => node.name)));
    } 

    private void ResetNodes(){
        foreach (PathNode node in nodes)
        {
            node.cost = float.MaxValue;
            node.heuristic = float.MaxValue;
            node.previous = null;
        }
    }


    private void fillGraph (){
        graph.Clear();
        foreach (PathNode node in nodes)
        {
            node.cost = float.MaxValue;
            node.heuristic = float.MaxValue;
            node.previous = null;
            graph.Add(node);
        }
    }

}
