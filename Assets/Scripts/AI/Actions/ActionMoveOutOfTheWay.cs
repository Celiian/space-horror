using UnityEngine;
using BehaviorTree;
using UnityEditor.ShaderGraph.Internal;

public class ActionMoveOutOfTheWay : Node
{
    private Transform transform;
    private Rigidbody2D rb;
    private LayerMask layerMask;
    private float timeSinceLastMove = 0;
    public ActionMoveOutOfTheWay(Transform transform, Rigidbody2D rb, LayerMask layerMask){
        this.transform = transform;
        this.rb = rb;
        this.layerMask = layerMask;
    }

    public override NodeState Evaluate()
    {   
        Transform target = (Transform)GetData("target");
        if(target == null){
            return NodeState.FAILURE;
        }

        timeSinceLastMove += Time.fixedDeltaTime;
        if(timeSinceLastMove > 0.6f){
            timeSinceLastMove = 0;
            SetTopParentData("cannotMove", true);
            return NodeState.SUCCESS;
        }
        else {
            float speed = (float)GetData("speed");
            // Get the perpendicular direction to the target
            Vector2 direction = target.position - transform.position;
            Vector2 rightDirection = Vector2.Perpendicular(direction).normalized;
            if (Vector2.Dot(rightDirection, Vector2.up) < 0) {
                rightDirection = -rightDirection;
            }
            rb.velocity = rightDirection * speed;
        }

        return NodeState.FAILURE;
    }
}
