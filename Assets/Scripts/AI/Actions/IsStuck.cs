using UnityEngine;
using BehaviorTree;

public class IsStuck : Node
{
    private Transform transform;
    private Vector3 lastPosition;
    private float stuckTime;
    private const float stuckThreshold = 0.25f;

    public IsStuck(Transform transform)
    {
        this.transform = transform;
        lastPosition = transform.position;
        stuckTime = 0f;
    }

    public override NodeState Evaluate()
    {
        bool isStuck = (bool)GetData("stuck");
        if (isStuck) {
            lastPosition = transform.position;
            return NodeState.SUCCESS;
        }

        if (lastPosition == transform.position)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime >= stuckThreshold)
            {
                SetTopParentData("stuck", true);
                stuckTime = 0f;
            }
        }

        lastPosition = transform.position;
        return NodeState.SUCCESS;
    }
}
