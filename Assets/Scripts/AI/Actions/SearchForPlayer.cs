using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

public class SearchForPlayer : Node
{
    private Transform transform;
    private List<PathNode> pathWaypoints;
    private int currentWaypointIndex = 0;
    private Vector3 originalDirection;
    public SearchForPlayer(Transform transform)
    {
        this.transform = transform;
    }

    public override NodeState Evaluate()
    {
        if (IsPlayerLost())
        {
            InitializeSearch();
        }

        UpdateTargetWaypoint();

        if (IsCloseToCurrentWaypoint())
        {
            if (IsAtLastWaypoint())
            {
                return InitializeAdvancedSearch();
            }

            currentWaypointIndex++;
            return NodeState.RUNNING;
        }

        return NodeState.RUNNING;
    }

    private bool IsPlayerLost()
    {
        return (bool)GetData("playerLost");
    }

    private void InitializeSearch()
    {
        Debug.Log("Initialize search");
        Transform lastPlayerPosition = (Transform)GetData("target");
        originalDirection = (lastPlayerPosition.position - transform.position).normalized;
        PathNode startNode = PathFinding.Instance.FindNodeCloseToPosition(transform.position);
        PathNode endNode = PathFinding.Instance.FindNodeCloseToPosition(lastPlayerPosition.position);
        Debug.Log("Start node: " + startNode.transform.position);
        Debug.Log("End node: " + endNode.transform.position);
        pathWaypoints = PathFinding.Instance.GetRandomPath(start: startNode, end: endNode);
        SetTopParentData("target", pathWaypoints[0].transform);
        SetTopParentData("playerLost", false);
        SetTopParentData("searchForPlayer", true);
        currentWaypointIndex = 0;
    }
    private void UpdateTargetWaypoint()
    {
        PathNode currentWaypoint = pathWaypoints[currentWaypointIndex];
        if (pathWaypoints.Count > 0)
        {
            if (currentWaypoint != null)
                SetTopParentData("target", currentWaypoint.transform);
        }
    }

    private bool IsAtLastWaypoint()
    {
        return currentWaypointIndex == pathWaypoints.Count - 1;
    }

    private NodeState InitializeAdvancedSearch()
    {
        if (!(bool)GetData("advancedSearchForPlayer"))
        {
            Debug.Log("Initialize advanced search");
            SetTopParentData("searchForPlayer", false);
            SetTopParentData("advancedSearchForPlayer", true);
            PathNode endNode = PathFinding.Instance.FindNodeCloseToPositionInDirection(transform.position, originalDirection);
            PathNode startNode = pathWaypoints[pathWaypoints.Count - 1];
            pathWaypoints = PathFinding.Instance.GetRandomPath(start: startNode, end: endNode);
            currentWaypointIndex = 0;
            return NodeState.SUCCESS;
        }
        else
        {
            Debug.Log("Advanced search failed");
            SetTopParentData("advancedSearchForPlayer", false);
            return NodeState.FAILURE;
        }
    }

    private bool IsCloseToCurrentWaypoint()
    {
        float distanceToTarget = Vector2.Distance(transform.position, pathWaypoints[currentWaypointIndex].transform.position);
        return distanceToTarget < 0.1f;
    }
}
