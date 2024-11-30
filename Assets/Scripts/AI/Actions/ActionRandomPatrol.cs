using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;

public class ActionRandomPatrol : Node
{
    private Transform transform;
    private List<PathNode> pathWaypoints;
    private int currentWaypointIndex = 0;

    public ActionRandomPatrol(Transform transform)
    {
        this.transform = transform;

        InitializePath();
    }

    public override NodeState Evaluate()
    {
        if (ShouldResetPath())
        {
            ResetPath();
            return NodeState.FAILURE;
        }

        if ((bool)GetData("playerLost"))
        {
            ResetPath();
            SetTopParentData("playerLost", false);
            return NodeState.FAILURE;
        }

        // Set target to the current waypoint
        PathNode currentWaypoint = pathWaypoints[currentWaypointIndex];
        SetTarget(currentWaypoint.transform);

        // Check distance to the target
        float distanceToTarget = Vector2.Distance(transform.position, currentWaypoint.transform.position);

        if (IsAtLastWaypoint())
        {
            RefreshPath(currentWaypoint);
            return NodeState.SUCCESS;
        }

        if (IsCloseToCurrentWaypoint(distanceToTarget))
        {
            currentWaypointIndex++;
            return NodeState.RUNNING;
        }

        return NodeState.RUNNING;
    }

    private void InitializePath()
    {
        PathNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        pathWaypoints = PathFinding.Instance.GetRandomPath(startNode);
    }

    private bool ShouldResetPath()
    {
        return pathWaypoints == null || pathWaypoints.Count == 0 || (bool)GetData("cannotMove") || pathWaypoints[currentWaypointIndex] == null;
    }

    private bool IsAtLastWaypoint()
    {
        return currentWaypointIndex == pathWaypoints.Count - 1;
    }

    private bool IsCloseToCurrentWaypoint(float distanceToTarget)
    {
        return distanceToTarget < 0.1f;
    }

    private void RefreshPath(PathNode currentWaypoint)
    {
        pathWaypoints = PathFinding.Instance.GetRandomPath(currentWaypoint);
        currentWaypointIndex = 0;
    }

    private void ResetPath()
    {
        SetTopParentData("cannotMove", false);
        InitializePath();
        currentWaypointIndex = 0;
    }

    private void SetTarget(Transform target)
    {
        SetTopParentData("target", target);
    }
}
