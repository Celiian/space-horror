using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
public class PathNodeManager : MonoBehaviour
{   
    public float maxDistanceBetweenNodes = 1.5f;
    public float maxNodeProcessingDistance = 6.0f;
    

    [Button]
    public void FillAllNodes(){
        List<PathNode> nodes = new List<PathNode>(FindObjectsOfType<PathNode>());
        foreach (PathNode node in nodes)
        {
            if (Vector3.Distance(node.transform.position, transform.position) <= maxNodeProcessingDistance)
            {
                node.FillNeighbours();
            }
        }

        foreach (PathNode node in nodes)
        {
            if (Vector3.Distance(node.transform.position, transform.position) <= maxNodeProcessingDistance)
            {
                node.DrawNeighbours();
            }
        }
    }

    [Button]
    private void MakeAllWallsBigger(){
        List<ScaleChanger> walls = new List<ScaleChanger>(FindObjectsOfType<ScaleChanger>());
        foreach (ScaleChanger wall in walls)
        {
            wall.GetComponent<BoxCollider2D>().size = new Vector2(wall.xLength + 0.6f, wall.yLength + 0.6f);
        }
    }


    [Button]
    private void ResetAllWalls(){
        List<ScaleChanger> walls = new List<ScaleChanger>(FindObjectsOfType<ScaleChanger>());
        foreach (ScaleChanger wall in walls)
        {
            wall.GetComponent<BoxCollider2D>().size = new Vector2(wall.xLength, wall.yLength);
        }
    }


    [Button]
    public void CreateIntermediateNodes(){
        List<PathNode> nodes = new List<PathNode>(FindObjectsOfType<PathNode>());
        foreach (PathNode node in nodes)
        {
            for (int i = 0; i < node.neighbours.Count; i++)
            {
                PathNode.Neighbour neighbour = node.neighbours[i];
                float distance = Vector3.Distance(node.transform.position, neighbour.node.transform.position);
                
                if (distance > maxDistanceBetweenNodes)
                {
                    Vector3 direction = (neighbour.node.transform.position - node.transform.position).normalized;
                    int numNewNodes = Mathf.FloorToInt(distance / maxDistanceBetweenNodes);
                    
                    for (int j = 1; j <= numNewNodes; j++)
                    {
                        Vector3 newPosition = node.transform.position + direction * maxDistanceBetweenNodes * j;
                        Instantiate(node, newPosition, Quaternion.identity);
                    }
                }
            }   
        }
    }
}
