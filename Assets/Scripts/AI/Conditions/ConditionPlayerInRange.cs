using BehaviorTree;
using UnityEngine;

public class ConditionPlayerInRange : Node
{
    private Transform transform;

    public ConditionPlayerInRange(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Checking if player is in range");
        Transform player = GameManager.Instance.player.transform;
        float visionLength = (float)GetData("visionLength");
        float DistanceToTarget = CalcUtils.DistanceToTarget(transform, player);
        
        if(DistanceToTarget < visionLength){
            Debug.Log("Player is in range");
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
