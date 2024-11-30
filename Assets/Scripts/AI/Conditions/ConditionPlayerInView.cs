using BehaviorTree;
using UnityEngine;

public class ConditionPlayerInView : Node
{
    private Transform transform;

    public ConditionPlayerInView(Transform transform){
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        float visionAngle = (float)GetData("visionAngle");
        Transform player = GameManager.Instance.player.transform;

        Vector2 directionToTarget = (player.position - transform.position).normalized;
        float angle = Vector2.Angle(transform.right, directionToTarget);
        if(angle < visionAngle / 2){
            Debug.Log("Player is in view");
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }

    private void DrawVisionCone(){
        float visionAngle = (float?)GetData("visionAngle") ?? 0;
        float detectionRadius = (float?)GetData("detectionRadius") ?? 0;
        float leftAngle = -visionAngle / 2;
        float rightAngle = visionAngle / 2;

        Vector2 leftDirection = Quaternion.Euler(0, 0, leftAngle) * transform.right;
        Debug.DrawRay(transform.position, leftDirection * detectionRadius, Color.red);

        Vector2 rightDirection = Quaternion.Euler(0, 0, rightAngle) * transform.right;
        Debug.DrawRay(transform.position, rightDirection * detectionRadius, Color.red);
    }

}
