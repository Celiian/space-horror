using BehaviorTree;

public class CheckFloatEqual : Node {
    private string condition;
    private float value;

    public CheckFloatEqual(string condition, float value) {
        this.condition = condition;
        this.value = value;
    }

    public override NodeState Evaluate() {
        float? conditionValue = (float?)GetData(condition);
        if (conditionValue == null || conditionValue.Value != value) {
            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }
}
