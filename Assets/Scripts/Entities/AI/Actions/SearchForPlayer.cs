using BehaviorTree;
using UnityEngine;
public class SearchForPlayer : Node
{
    private Transform transform;

    public SearchForPlayer(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        Vector3 playerLastKnownPosition = (Vector3)GetData("lastKnownPlayerPosition");

        if (HasReached(playerLastKnownPosition)) {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }

    private bool HasReached(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < 0.1f;
    }


}
