using BehaviorTree;
using UnityEngine;
public class CheckIntEqual : Node {
    private string condition;
    private int value;

    public CheckIntEqual(string condition, int value) {
        this.condition = condition;
        this.value = value;
    }

    public override NodeState Evaluate() {
        int? conditionValue = (int?)GetData(condition);
        if (conditionValue == null || conditionValue.Value != value) {
            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }
}
