using BehaviorTree;
using System.Collections.Generic;
using UnityEngine;
public class SetData : Node {
    private Dictionary<string, object> data;

    public SetData(Dictionary<string, object> data) {
        this.data = data;
    }

    public SetData(Dictionary<string, float> data) {
        this.data = new Dictionary<string, object>();
        foreach (var kvp in data) {
            this.data[kvp.Key] = kvp.Value;
        }
    }

    public override NodeState Evaluate() {
        foreach (var kvp in data) {
            SetTopParentData(kvp.Key, kvp.Value);
        }
        return NodeState.SUCCESS;
    }

    public override NodeState EvaluateFixedUpdate()
    {
        foreach (Node child in children)
        {
            child.EvaluateFixedUpdate();
        }
        return NodeState.RUNNING;
    }

    public override NodeState EvaluateLateUpdate()
    {
        foreach (Node child in children)
        {
            child.EvaluateLateUpdate();
        }
        return NodeState.RUNNING;
    }
}