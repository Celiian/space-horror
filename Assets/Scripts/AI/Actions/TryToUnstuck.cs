using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
public class TryToUnstuck : Node
{
    private Transform transform;
    private List<TileNode> path;

    private int currentNodeIndex = 0;

    public TryToUnstuck(Transform transform){
        this.transform = transform;
        path = new List<TileNode>();
        currentNodeIndex = 0;
    }


    public override NodeState Evaluate()
    {   
        if(path.Count == 0){
            FindPathToPlayer();
        }

        if(currentNodeIndex == 1){
            SetTopParentData("stuck", false);
            path = new List<TileNode>();
            currentNodeIndex = 0;
            return NodeState.SUCCESS;
        }

        if(CloseToCurrentTarget()){
            currentNodeIndex++;
            if(currentNodeIndex >= path.Count){
                currentNodeIndex = 0;
            }
            SetTopParentData("currentTargetPosition", path[currentNodeIndex].GetPosition());
            return NodeState.SUCCESS;
        }
        return NodeState.RUNNING;
    }

    private bool CloseToCurrentTarget(){    
        Vector3 currentTargetPosition = (Vector3)GetData("currentTargetPosition");
        if(currentTargetPosition == null){
            return false;
        }
        return CalcUtils.DistanceToTarget(transform.position, currentTargetPosition) < 0.2f;
    }


    private void FindPathToPlayer(){
        currentNodeIndex = 0;
        Vector3 playerPosition = PlayerMovement.Instance.transform.position;
        var startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        var endNode = PathFinding.Instance.FindNodeCloseToPosition(playerPosition);
        path = PathFinding.Instance.FindPath(startNode, endNode);
        SetTopParentData("currentTargetPosition", path[currentNodeIndex].GetPosition());
    }
}
