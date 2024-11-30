using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// Represents the possible states of a node in a behavior tree.
    /// </summary>
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }


    /// <summary>
    /// Represents a node in a behavior tree.
    /// </summary>
    public class Node
    {
        protected NodeState _state;

        public Node parent;

        protected List<Node> children;

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        public Node()
        {
            parent = null;
            children = new List<Node>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class with specified children.
        /// </summary>
        /// <param name="children">The list of child nodes.</param>
        public Node(List<Node> children)
        {
            parent = null;
            this.children = new List<Node>();
            foreach (Node node in children)
            {
                _Attach(node);
            }
        }


        /// <summary>
        /// Attaches a child node to this node.
        /// </summary>
        /// <param name="node">The child node to attach.</param>
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        /// <summary>
        /// Evaluates the node's state.
        /// </summary>
        /// <returns>The current state of the node.</returns>
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public virtual NodeState EvaluateLateUpdate() => NodeState.RUNNING;

        public virtual NodeState EvaluateFixedUpdate() => NodeState.RUNNING;


        /// <summary>
        /// Sets data in the node's data context.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <param name="value">The value to set.</param>
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        /// <summary>
        /// Sets data in the top parent node's data context.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <param name="value">The value to set.</param>
        public void SetTopParentData(string key, object value)
        {
            if (parent == null)
            {
                SetData(key, value);
            }
            else
            {
                parent.SetTopParentData(key, value);
            }
        }


        /// <summary>
        /// Gets data from the node's data context or its parents.
        /// </summary>
        /// <param name="key">The key for the data.</param>
        /// <returns>The value associated with the key, or null if not found.</returns>
        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;
            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        /// <summary>
        /// Clears data from the node's data context or its parents.
        /// </summary>
        /// <param name="key">The key for the data to clear.</param>
        /// <returns>True if the data was cleared; otherwise, false.</returns>
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }

            return false;
        }
    }

}

