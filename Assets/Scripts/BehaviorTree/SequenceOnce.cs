using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// Represents a node that runs all its children once and then always returns FAILURE.
    /// </summary>
    public class SequenceOnce : Node
    {
        private bool _hasRun;

        /// <summary>
        /// Initializes a new instance of the SequenceOnce class with no children.
        /// </summary>
        public SequenceOnce() : base() 
        {
            _hasRun = false;
        }

        /// <summary>
        /// Initializes a new instance of the SequenceOnce class with the specified children.
        /// </summary>
        /// <param name="children">The list of child nodes to be evaluated.</param>
        public SequenceOnce(List<Node> children) : base(children) 
        {
            _hasRun = false;
        }

        public override NodeState Evaluate()
        {
            if(_hasRun){
                _state = NodeState.FAILURE;
                return _state;
            }

            // Check if the children have already been executed
            if (!_hasRun)
            {
                foreach (Node child in children)
                {
                    // Evaluate each child node
                    child.Evaluate();
                }

                // Set the flag to indicate that children have run
                _hasRun = true;
            }

            _state = NodeState.SUCCESS;
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
