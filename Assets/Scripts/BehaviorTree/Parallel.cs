using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// Represents a parallel node in a behavior tree.
    /// A parallel node evaluates all its children simultaneously.
    /// - Succeeds if all children succeed.
    /// - Fails if any child fail but none if running.
    /// - Keeps running if at least one child is running and none have failed.
    /// </summary>
    public class Parallel : Node
    {
        public Parallel() : base() { }
        public Parallel(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        _state = NodeState.FAILURE;
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        break;
                    case NodeState.SUCCESS:
                        if(_state != NodeState.FAILURE)
                            _state = NodeState.SUCCESS;
                        continue;
                }
            }

            // If any child is running, the parallel node is still running.
            _state = anyChildIsRunning ? NodeState.RUNNING : _state;
            return _state;
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
}
