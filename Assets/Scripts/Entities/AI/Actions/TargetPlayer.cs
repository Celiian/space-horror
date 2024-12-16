using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class TargetPlayer : Node
{
    private Transform transform;
    private List<TileNode> followPoints;
    private int currentFollowPointIndex = 0;

    public TargetPlayer(Transform transform)
    {
        this.transform = transform;
        followPoints = new List<TileNode>();
    }

    public override NodeState Evaluate()
    {
        var playerPosition = PlayerMovement.Instance.transform.position;
        
        if (followPoints == null || followPoints.Count == 0) {
            InitializeFollowPoints(playerPosition);
            return NodeState.RUNNING;
        }

        var currentWayPoint = (Vector3)GetData("currentPlayerPosition");

        if(HasReachedDestination(currentWayPoint) && followPoints.Count > 1){
            ContinueFollowingPlayer(playerPosition);
            Debug.Log("Continue following player");
            return NodeState.RUNNING;
        }

        SetTopParentData("currentPlayerPosition", followPoints[currentFollowPointIndex].GetPosition());
        return NodeState.SUCCESS;
    }

    private bool HasReachedDestination(Vector3 currentWayPoint){
        var distance = Vector3.Distance(transform.position, currentWayPoint);
        return distance < 0.1f;
    }

    private void ContinueFollowingPlayer(Vector3 playerPosition){
        var startingNode = PathFinding.Instance.FindNodeCloseToPosition(followPoints[currentFollowPointIndex + 1].GetPosition());
        var destinationNode = PathFinding.Instance.FindNodeCloseToPosition(playerPosition);
        if (startingNode != null) {
            followPoints = PathFinding.Instance.FindPath(startingNode, destinationNode);
            if (followPoints == null || followPoints.Count == 0) {
                followPoints = new List<TileNode> { startingNode };
            }
        }
        currentFollowPointIndex = 0;
        SetTopParentData("currentPlayerPosition", followPoints[currentFollowPointIndex].GetPosition());
    }

    private void InitializeFollowPoints(Vector3 playerPosition){
        currentFollowPointIndex = 0;
        var startingNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        var destinationNode = PathFinding.Instance.FindNodeCloseToPosition(playerPosition);
        if (startingNode != null) {
            followPoints = PathFinding.Instance.FindPath(startingNode, destinationNode);
            if (followPoints == null || followPoints.Count == 0) {
                followPoints = new List<TileNode> { startingNode };
            }
        }
        SetTopParentData("currentPlayerPosition", followPoints[currentFollowPointIndex].GetPosition());
    }
}