using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class RandomPatrol : Node
{
    private Transform transform;
    private List<TileNode> patrolPoints;
    private int currentPatrolPointIndex = 0;

    public RandomPatrol(Transform transform)
    {
        this.transform = transform;
        patrolPoints = new List<TileNode>();
    }

    public override NodeState Evaluate()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) {
            InitializePatrolPoints();
            return NodeState.FAILURE;
        }

        var currentWayPoint = (Vector3)GetData("currentPatrolPoint");
        if(TooFarFromDestination(currentWayPoint)){
            RecalculatePatrolPoints();
        }

        if(HasReachedDestination(currentWayPoint)){
            currentPatrolPointIndex++;

            if(currentPatrolPointIndex >= patrolPoints.Count){
                InitializePatrolPoints();
            }
        }

        SetTopParentData("currentPatrolPoint", patrolPoints[currentPatrolPointIndex].GetPosition());
        return NodeState.SUCCESS;
    }

    private bool TooFarFromDestination(Vector3 currentWayPoint){
        var distance = Vector3.Distance(transform.position, currentWayPoint);
        return distance > 2f;
    }

    private bool HasReachedDestination(Vector3 currentWayPoint){
        var distance = Vector3.Distance(transform.position, currentWayPoint);
        return distance < 0.1f;
    }

    private void InitializePatrolPoints(){
        currentPatrolPointIndex = 0;
        var startingNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        if (startingNode != null) {
            patrolPoints = PathFinding.Instance.GetRandomPath(startingNode);
            if (patrolPoints == null || patrolPoints.Count == 0) {
                // Fallback if no path is found
                patrolPoints = new List<TileNode> { startingNode };
            }
        }
        SetTopParentData("currentPatrolPoint", patrolPoints[currentPatrolPointIndex].GetPosition());
    }

    private void RecalculatePatrolPoints(){
        currentPatrolPointIndex = 0;
        var startingNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        if (startingNode != null && patrolPoints != null && patrolPoints.Count > 0) {
            var destinationNode = patrolPoints[patrolPoints.Count - 1];
            var newPath = PathFinding.Instance.GetRandomPath(startingNode, destinationNode);
            if (newPath != null && newPath.Count > 0) {
                patrolPoints = newPath;
            }
        }
    }
}
