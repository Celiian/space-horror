using BehaviorTree;
using UnityEngine;
public class CheckNull : Node {
    private string condition;

    public CheckNull(string condition) {
        this.condition = condition;
    }

    public override NodeState Evaluate() {
        object conditionValue = GetData(condition);
        if (conditionValue == null ) {
            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }
}
