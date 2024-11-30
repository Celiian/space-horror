using BehaviorTree;
using UnityEngine;

public class ActionMoveToTarget : Node
{
    private Transform transform;
    private Rigidbody2D rb;
    public ActionMoveToTarget(Transform transform, Rigidbody2D rb){
        this.transform = transform;
        this.rb = rb;
    }


    public override NodeState Evaluate()
    {
        MoveToTarget();
        return NodeState.SUCCESS;
    }


    private void MoveToTarget()
    {
        Transform target = (Transform)GetData("target");
        float speed = (float?)GetData("speed") ?? 0;

        if(target == null){
            return;
        }


        Vector2 direction = (target.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rb.rotation = Mathf.LerpAngle(rb.rotation, targetAngle, 50 * Time.fixedDeltaTime);
        rb.velocity = direction * speed;
    }
}
