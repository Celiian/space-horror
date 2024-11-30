using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;
using BehaviorTree;

public class PathNode : MonoBehaviour
{
    public LayerMask layerMask;
    public List<Neighbour> neighbours = new List<Neighbour>();
    public float fillRadius = 40f;
    public float heuristic;
    public float cost;
    public PathNode previous;

    
    [System.Serializable]
    public class Neighbour
    {
        public PathNode node;
        public Vector3 direction;
    }


    [Button]
    public void DrawNeighbours(){
        foreach (Neighbour neighbour in neighbours)
        {
            StartCoroutine(DrawNeighboursCoroutine(neighbour));
        }
    }


    public IEnumerator DrawNeighboursCoroutine(Neighbour neighbour){
        int i = 0;
        while (i < 10)
        {
            Debug.DrawLine(transform.position, neighbour.node.transform.position, Color.red);
            yield return new WaitForSeconds(0.1f);
            i++;
        }
    }   




    [Button]
    public void FillNeighbours(float radius = 40f){
        neighbours.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("PathNode") && collider.transform != transform){
                Vector3 direction = (collider.transform.position - transform.position).normalized;
                RayCastNeighbours(direction);
            }
        }
        // Vector3[] allDirections = {
        //     Vector3.up,
        //     Vector3.down,
        //     Vector3.left,
        //     Vector3.right,
        //     Vector3.up + Vector3.right,
        //     Vector3.up + Vector3.left,
        //     Vector3.down + Vector3.right,
        //     Vector3.down + Vector3.left
        // };

        // foreach (Vector3 direction in allDirections)
        // {
        //     RayCastNeighbours(direction);
        // }
    }

    private void RayCastNeighbours(Vector3 direction){
        RaycastHit2D hit = Physics2D.Raycast(transform.position + direction.normalized, direction, fillRadius, layerMask);
        if(hit.collider.CompareTag("Wall")){
            return;
        }
        else{
            PathNode hitNode = hit.collider.GetComponent<PathNode>();
            bool alreadyExists = neighbours.Exists(n => n.node == hitNode || n.direction == direction);
            if (!alreadyExists) {
                Neighbour neighbour = new Neighbour { node = hitNode, direction = direction };
                neighbours.Add(neighbour);
            }
        }
    }

}