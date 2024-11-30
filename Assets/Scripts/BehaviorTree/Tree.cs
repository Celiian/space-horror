using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        public Node _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            if(_root == null) return;
            _root.Evaluate();
        }

        protected abstract Node SetupTree();

    }
}
