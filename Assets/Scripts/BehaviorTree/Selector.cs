using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// Represents a selector node in a behavior tree.
    /// A selector node evaluates its children until one succeeds or all fail.
    /// </summary>
    public class Selector : Node
    {
        /// <summary>
        /// Initializes a new instance of the Selector class with no children.
        /// </summary>
        public Selector() : base() { }

        /// <summary>
        /// Initializes a new instance of the Selector class with the specified children.
        /// </summary>
        /// <param name="children">The list of child nodes to be evaluated.</param>
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        _state = NodeState.SUCCESS;
                        return _state;
                    case NodeState.RUNNING:
                        _state = NodeState.RUNNING;
                        return _state;
                    default:
                        continue;
                }
            }

            _state = NodeState.FAILURE;
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