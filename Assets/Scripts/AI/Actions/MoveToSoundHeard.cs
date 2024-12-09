using UnityEngine;
using BehaviorTree;
using System.Collections.Generic;

public class MoveToSoundHeard : Node
{
    private Transform _transform;
    private List<TileNode> _path;
    private int _currentNodeIndex;
    private Vector3 previousSoundPosition;

    public MoveToSoundHeard(Transform transform)
    {
        _transform = transform;
        _path = new List<TileNode>();
        _currentNodeIndex = 0;
    }

    public override NodeState Evaluate()
    {
        Vector3 soundPosition = (Vector3)GetData("soundPosition");

        if (_path.Count == 0 || Vector3.Distance(soundPosition, previousSoundPosition) > 2f)
        {
            FindPathToSound(soundPosition);
        }

        Vector3 targetPosition = _path[_currentNodeIndex].GetPosition();
        SetTopParentData("currentSoundTargetPosition", targetPosition);

        if (_currentNodeIndex >= _path.Count)
        {
            return NodeState.SUCCESS;
        }

        if (CloseToCurrentNode())
        {
            _currentNodeIndex++;
            if (_currentNodeIndex >= _path.Count)
            {
                return NodeState.SUCCESS;
            }
        }

        return NodeState.SUCCESS;
    }

    private bool CloseToCurrentNode()
    {
        return Vector3.Distance(_transform.position, _path[_currentNodeIndex].GetPosition()) < 0.1f;
    }

    private void FindPathToSound(Vector3 soundPosition)
    {
        var startNode = PathFinding.Instance.FindNodeCloseToPosition(_transform.position);
        var endNode = PathFinding.Instance.FindNodeCloseToPosition(soundPosition);
        _path = PathFinding.Instance.FindPath(startNode, endNode);
        _currentNodeIndex = 0;

        Vector3 targetPosition = _path[_currentNodeIndex].GetPosition();
        SetTopParentData("currentSoundTargetPosition", targetPosition);
    }

}