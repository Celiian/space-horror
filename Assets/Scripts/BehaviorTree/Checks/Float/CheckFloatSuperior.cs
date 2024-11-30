using BehaviorTree;
using UnityEngine;

public class CheckFloatSuperior : Node {
    private string condition;
    private float value;

    public CheckFloatSuperior(string condition, float value) {
        this.condition = condition;
        this.value = value;
    }

    public override NodeState Evaluate() {

        float? conditionValue = (float?)GetData(condition);
        if (conditionValue != null && conditionValue.Value > value) {
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
