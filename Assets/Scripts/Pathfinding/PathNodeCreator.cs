using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum CardinalDirection
{
    North,
    East,
    South,
    West
}

public class PathNodeCreator : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float fillDistance = 4;
    [SerializeField] private PathNode nodePrefab;
    [EnumToggleButtons]
    [SerializeField] private CardinalDirection direction;

    
    [Button]
    public void AddNeighbour(int amount = 1){
        for(int i = 0; i < amount; i++){
            Vector3 directionVector = GetDirectionVector(direction) * (i + 1);
            RaycastHit2D hit = Physics2D.Raycast(parent.position + directionVector.normalized, directionVector.normalized, fillDistance * (i + 1), layerMask);
            if(hit.collider != null && hit.collider.CompareTag("Wall")){
                break;
            }
            else{
                Instantiate(nodePrefab, parent.position + directionVector, Quaternion.identity);
            }
        }
    }


    [Button]
    public void FillAllNodes(){
        List<PathNode> nodes = new List<PathNode>(FindObjectsOfType<PathNode>());
        foreach (PathNode node in nodes)
        {
            node.FillNeighbours(fillDistance * 2);
        }

        foreach (PathNode node in nodes)
        {
            node.DrawNeighbours();
        }
    }
    private Vector3 GetDirectionVector(CardinalDirection direction){
        return direction switch
        {
            CardinalDirection.North => Vector3.up * fillDistance,
            CardinalDirection.East => Vector3.right * fillDistance,
            CardinalDirection.South => Vector3.down * fillDistance,
            CardinalDirection.West => Vector3.left * fillDistance,
        };
    }
}
