using UnityEngine;
using BehaviorTree;

public class ConditionIsNotMoving : Node
{
    private Rigidbody2D rb;
    public ConditionIsNotMoving(Rigidbody2D rb){
        this.rb = rb;
    }

    public override NodeState Evaluate()
    {
        if(rb.velocity.magnitude < 0.3f){
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}