using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// Represents a sequence node in a behavior tree.
    /// A sequence node evaluates its children in order until one fails or all succeed.
    /// </summary>
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        _state = NodeState.FAILURE;
                        return _state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        break;
                }
            }

            _state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
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