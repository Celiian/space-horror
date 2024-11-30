using BehaviorTree;
using UnityEngine;

public class CheckIntSuperior : Node {
    private string condition;
    private int value;

    public CheckIntSuperior(string condition, int value) {
        this.condition = condition;
        this.value = value;
    }

    public override NodeState Evaluate() {

        int? conditionValue = (int?)GetData(condition);
        if (conditionValue == null || conditionValue.Value <= value) {

            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }
}
