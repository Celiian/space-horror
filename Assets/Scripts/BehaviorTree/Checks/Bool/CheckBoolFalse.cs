using BehaviorTree;
using UnityEngine;

public class CheckBoolFalse : Node {
    private string condition;

    public CheckBoolFalse(string condition) {
        this.condition = condition;
    }

    public override NodeState Evaluate() {

        bool? conditionValue = (bool?)GetData(condition);
        if (conditionValue != null && conditionValue.Value) {
            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }
}
